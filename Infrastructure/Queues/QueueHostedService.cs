using OrderService.Constants.Logger;

namespace OrderService.Infrastructure.Queues {

    public class QueuedHostedService(
        BackgroundTaskQueue taskQueue,
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider
    ) : BackgroundService
    {

        private readonly BackgroundTaskQueue TaskQueue = taskQueue;
        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ACTIVITY);
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = 
                    await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}