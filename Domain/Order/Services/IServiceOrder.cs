using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Product.Dtos;
using OrderService.Infrastructure.Dtos;

namespace OrderService.Domain.Order.Services
{
    public interface IServiceOrder
    {
        Task<PaginationModel<OrderResultDto>> FindAllAsync(OrderQueryDto param);
        Task<PaginationModel<OrderDetailResultDto>> FindOrderDetailAsync(Guid id, ProductQueryDto param);
        Task<string> CreateOrderAsync(CreateOrderDto body);
    }
}