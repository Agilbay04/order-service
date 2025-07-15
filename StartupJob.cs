using OrderService.Infrastructure.Jobs;

namespace OrderService
{
    public partial class Startup
    {
        public void Jobs(IServiceCollection services)
        {
            services.AddScoped<NotificationHouseKeepingJob>();
        }
    }
}
