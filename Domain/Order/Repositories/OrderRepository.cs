using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Order.Constants;
using OrderService.Infrastructure.Databases;

namespace OrderService.Domain.Order.Repositories
{
    public class OrderRepository(DataContext dbContext)
    {
        private readonly DataContext _dbContext = dbContext;

        public async Task<Models.Order> GetOrderById(Guid orderId)
        {
            return await _dbContext.Orders
                .Where(o => o.Id == orderId
                    && o.DeletedAt == null
                    && o.Status == OrderStatusConstant.PENDING)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetLatestOrderNumber(string prefix)
        {
            return await _dbContext.Orders
                .Where(o => o.OrderNumber.StartsWith(prefix))
                .OrderByDescending(o => o.OrderNumber)
                .Select(o => o.OrderNumber)
                .FirstOrDefaultAsync();
        }   
    }
}