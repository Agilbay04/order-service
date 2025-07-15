using OrderService.Infrastructure.BackgroundHosted;
using OrderService.Infrastructure.Integrations.Http;
using OrderService.Infrastructure.Integrations.NATs;

namespace OrderService
{
    public partial class Startup
    {
        public void Integrations(IServiceCollection services)
        {
            services.AddScoped<HttpIntegration>();
            services.AddSingleton<NATsIntegration>();
            services.AddSingleton<NATsTask>();
        }
    }
}
