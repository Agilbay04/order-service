using OrderService.Domain.Order.Repositories;

namespace OrderService
{
    public partial class Startup
    {
        public void Repositories(IServiceCollection services)
        {
            services.AddScoped<OrderRepository>();
        }
    }
}
