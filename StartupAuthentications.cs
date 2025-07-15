using OrderService.Infrastructure.Databases;

namespace OrderService
{
    public partial class Startup
    {
        public void Authentications(IServiceCollection services)
        {
            services.AddSingleton<LocalStorageDatabase>();
        }
    }
}
