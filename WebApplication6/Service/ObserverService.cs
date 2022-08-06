using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private readonly MotionDetection _motionDetection;



        public ObserverService(IOptions<ObserverOptions> options, INotificationService notificationService, IIpProvider ipProvider, IPictureProvider pictureProvider, KeyStorage keyStorage, MotionDetection motionDetection)
        {
            _options = options.Value;
            _notificationService = notificationService;
            _iIpprovider = ipProvider;
            _pictureProvider = pictureProvider;
            _keyStorage = keyStorage;
            _motionDetection = motionDetection;
        }



        public async Task Observe(CancellationToken stoppingToken)
        {
            var startTime = DateTime.UtcNow - TimeSpan.FromMinutes(_options.ObserveTimeoutInMinutes);
            var alarmEnabled = false;
            while (true)
            {

                if (_motionDetection.Detect())
                {
                    if (!alarmEnabled)
                    {
                        await Notify("MOTION IS DETECTED!!!!");
                        alarmEnabled = true;
                    }
                }
                else
                {
                    if (alarmEnabled)
                    {
                        await Notify("MOTION STOPS");
                        alarmEnabled = false;
                    }
                }

                if (DateTime.UtcNow - startTime > TimeSpan.FromMinutes(_options.ObserveTimeoutInMinutes))
                {
                    _keyStorage.Key = Guid.NewGuid().ToString("n"); ;
                    await Notify($"https://{await _iIpprovider.GetMyIpAsync(stoppingToken) ?? "address not available"}:{_options.OutsidePort}/api/stream?k={_keyStorage.Key}");
                    startTime = DateTime.UtcNow;
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

        }

        private async Task Notify(string message)
        {
            var pictureStream = _pictureProvider.GetPngPicture();
            await _notificationService.Notify(message, pictureStream);
        }
    }
}
