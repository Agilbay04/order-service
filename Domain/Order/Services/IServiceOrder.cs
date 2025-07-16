using OrderService.Domain.Order.Dtos;
using OrderService.Infrastructure.Dtos;

namespace OrderService.Domain.Order.Services
{
    public interface IServiceOrder
    {
        Task<PaginationModel<OrderResultDto>> FindAllAsync(OrderQueryDto param);
        Task<string> CreateOrder(CreateOrderDto body);
    }
}