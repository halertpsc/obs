using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication6.Providers;

namespace WebApplication6.Service
{
    public class ObserverService : IObserverService
    {
        private readonly ObserverOptions _options;
        private readonly INotificationService _notificationService;
        private readonly IIpProvider _iIpprovider;
        private readonly IPictureProvider _pictureProvider;
        private readonly KeyStorage _keyStorage;
        private readonly IMotionDetection _motionDetection;
        private readonly ILogger<ObserverService> _logger;



        public ObserverService(IOptions<ObserverOptions> options, INotificationService notificationService, IIpProvider ipProvider, IPictureProvider pictureProvider, KeyStorage keyStorage, IMotionDetection motionDetection, ILogger<ObserverService> logger)
        {
            _options = options.Value;
            _notificationService = notificationService;
            _iIpprovider = ipProvider;
            _pictureProvider = pictureProvider;
            _keyStorage = keyStorage;
            _motionDetection = motionDetection;
            _logger = logger;
        }

        public async Task Observe(CancellationToken stoppingToken)
        {
            await NotifyScheduled(stoppingToken);
            var startTime = DateTime.UtcNow;
            var alarmEnabled = false;
            while (true)
            {
                try
                {
                    if (_motionDetection.Detect())
                    {
                        if (!alarmEnabled)
                        {
                            await NotifyDetection("MOTION IS DETECTED!!!!");
                            alarmEnabled = true;
                        }
                    }
                    else
                    {
                        if (alarmEnabled)
                        {
                            await NotifyDetection("MOTION STOPS");
                            alarmEnabled = false;
                        }
                    }

                    if (DateTime.UtcNow - startTime > TimeSpan.FromMinutes(_options.ObserveTimeoutInMinutes))
                    {
                        await NotifyScheduled(stoppingToken);
                        startTime = DateTime.UtcNow;
                    }


                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Observing task was cancelled");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private async Task NotifyScheduled(CancellationToken stoppingToken)
        {
            _keyStorage.Key = Guid.NewGuid().ToString("n"); ;
            var pictureStream = _pictureProvider.GetPngPicture();
            await _notificationService.Notify("SCHEDULED NOTIFICATION", $"https://{await _iIpprovider.GetMyIpAsync(stoppingToken) ?? "address not available"}:{_options.OutsidePort}/api/stream?k={_keyStorage.Key}", pictureStream);
        }

        private async Task NotifyDetection(string message)
        {
            var pictureStream = _pictureProvider.GetPngPicture();
            await _notificationService.Notify(message, message, pictureStream);
        }
    }
}
