using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.File.Services;
using OrderService.Domain.Logging.Services;
using OrderService.Domain.Order.Services;
using OrderService.Domain.Product.Services;
using OrderService.Infrastructure.Shareds;

namespace OrderService
{
    public partial class Startup
    {
        public void Services(IServiceCollection services)
        {
            services.AddScoped<LoggingService>();
            services.AddScoped<StorageService>();
            services.AddScoped<FileService>();
            services.AddScoped<ServiceOrder>();
            services.AddScoped<IServiceOrder>(sp =>
            {
                var service = sp.GetRequiredService<ServiceOrder>();
                var cache = sp.GetRequiredService<IDistributedCache>();
                return new CachedServiceOrder(service, cache);
            });
            services.AddScoped<ProductService>();
        }
    }
}
