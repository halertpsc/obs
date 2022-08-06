using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication6.Providers;

namespace WebApplication6.Service
{
    public class ActivationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOwnerDetector _ownerDetector;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ActivationService> _logger;

        public ActivationService(IServiceProvider serviceProvider, IOwnerDetector ownerDetector, INotificationService notificationService, ILogger<ActivationService> logger)
        {
            _serviceProvider = serviceProvider;
            _ownerDetector = ownerDetector;
            _notificationService = notificationService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool ownerHere = true;
            CancellationTokenSource cancellationTokenSource = null;
            Task runningTask;
            while (true)
            {
                _logger.LogInformation($"Is owner detected : {ownerHere}");
                try
                {
                    if (ownerHere != await _ownerDetector.IsOwnerHere())
                    {
                        if (ownerHere)
                        {
                            ownerHere = false;
                            runningTask = Task.Run(async () =>
                            {
                                using var scope = _serviceProvider.CreateScope();
                                var observerService = scope.ServiceProvider.GetRequiredService<IObserverService>();
                                cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                                await observerService.Observe(cancellationTokenSource.Token);
                            });
                            await _notificationService.Notify("System armed. Observing started!!!", null);
                        }
                        else
                        {
                            ownerHere = true;
                            cancellationTokenSource?.Cancel();
                            await _notificationService.Notify("Owner detected. System disarmed.", null);
                        }

                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Error on attempt to detect owner presence: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}

