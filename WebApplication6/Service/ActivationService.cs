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
            _logger.LogInformation("Execute async starts.");
            CancellationTokenSource cancellationTokenSource = null;
            Task runningTask = Task.CompletedTask;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var ownerIsHere = await _ownerDetector.IsOwnerHere();
                    _logger.LogInformation("Owner here {ownerHere}, task status {taskStatus}", ownerIsHere, runningTask?.Status);
                    if (runningTask.IsCompleted && !ownerIsHere)
                    {
                        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                        runningTask = Task.Run(async ()=> await ExecuteInternal(cancellationTokenSource.Token), cancellationTokenSource.Token);
                        await _notificationService.Notify("SYSTEM ARMED", "System armed. Observing started!!!", null);
                    }
                    if (!runningTask.IsCompleted && ownerIsHere)
                    {
                        _logger.LogInformation("Cancelling observing task.");
                        cancellationTokenSource?.Cancel();
                        await _notificationService.Notify("OWNER DETECTED","Owner detected. System disarmed.", null);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            await runningTask;
        }

        private async Task ExecuteInternal(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting observing task.");
            using var scope = _serviceProvider.CreateScope();
            var observerService = scope.ServiceProvider.GetRequiredService<IObserverService>();
            await observerService.Observe(cancellationToken);
        }
    }
}

