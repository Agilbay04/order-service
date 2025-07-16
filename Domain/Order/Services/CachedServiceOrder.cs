using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.Order.Dtos;
using OrderService.Infrastructure.Dtos;
using System.Text.Json;

namespace OrderService.Domain.Order.Services
{
    public class CachedServiceOrder(
        IServiceOrder service,
        IDistributedCache cache
    ) : IServiceOrder
    {
        private readonly IServiceOrder _service = service;
        private readonly IDistributedCache _cache = cache;

        public async Task<PaginationModel<OrderResultDto>> FindAllAsync(OrderQueryDto param = null)
        {
            var cacheKey = $"orders:list:{JsonSerializer.Serialize(param)}";
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<PaginationModel<OrderResultDto>>(cached);
            }

            var result = await _service.FindAllAsync(param);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);

            return result;
        }

        public async Task<string> CreateOrder(CreateOrderDto body)
        {
            return await _service.CreateOrder(body);
        }
    }
}