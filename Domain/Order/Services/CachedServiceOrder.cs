using Microsoft.Extensions.Caching.Distributed;
using OrderService.Domain.Order.Constants;
using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Product.Dtos;
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
            var cacheKey = GenerateCahceKey(OrderRedisConstant.ORDER, OrderRedisConstant.LEVEL_LIST, param);
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var cahchedData = JsonSerializer.Deserialize<PaginationModel<OrderResultDto>>(cached);
                if (cahchedData != null && cahchedData.Data.Count > 0)
                {
                    return cahchedData;
                }
            }

            var result = await _service.FindAllAsync(param);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);

            return result;
        }

        public async Task<PaginationModel<OrderDetailResultDto>> FindOrderDetailAsync(Guid id, ProductQueryDto param = null)
        {
            var cacheKey = GenerateCahceKey(OrderRedisConstant.ORDER, OrderRedisConstant.LEVEL_DETAIL, param);
            var cached = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cached))
            {
                var cahchedData = JsonSerializer.Deserialize<PaginationModel<OrderDetailResultDto>>(cached);
                if (cahchedData != null && cahchedData.Data.Count > 0)
                {
                    return cahchedData;
                }
            }

            var result = await _service.FindOrderDetailAsync(id, param);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);

            return result;
        }

        public async Task<string> CreateOrderAsync(CreateOrderDto body)
        {
            return await _service.CreateOrderAsync(body);
        }

        private static string GenerateCahceKey<T>(string key, string level, T param)
        {
            return $"{key}:{level}:{JsonSerializer.Serialize(param)}";
        }
    }
}