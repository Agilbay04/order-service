using OrderService.Domain.Logging.Listeners;

namespace OrderService
{
    public partial class Startup
    {
        public void Listeners(IServiceCollection services)
        {
            services.AddScoped<LoggingNATsListener>();
            services.AddScoped<LoggingNATsListenAndReply>();
        }
    }
}
