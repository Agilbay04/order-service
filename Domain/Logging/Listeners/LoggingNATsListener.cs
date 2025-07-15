using OrderService.Constants.Logger;
using OrderService.Domain.Logging.Services;
using OrderService.Infrastructure.Shareds;
using OrderService.Infrastructure.Subscriptions;

namespace OrderService.Domain.Logging.Listeners
{
    public class LoggingNATsListener(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : ISubscriptionAction<IDictionary<string, object>>
    {
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public void Handle(IDictionary<string, object> data)
        {
            // EXAMPLE: Do operation event
            var jsonData = Utils.JsonSerialize(data);
            _logger.LogInformation(jsonData);
        }
    }
}
