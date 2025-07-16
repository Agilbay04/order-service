using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Order.Constants;
using OrderService.Domain.Order.Dtos;
using OrderService.Infrastructure.Databases;
using OrderService.Infrastructure.Dtos;
using OrderService.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace OrderService.Domain.Order.Repositories
{
    public class OrderRepository(DataContext dbContext)
    {
        private readonly DataContext _dbContext = dbContext;

        public async Task<PaginationResult<Models.Order>> PaginationAsync(OrderQueryDto queryParams)
        {
            int skip = (queryParams.Page - 1) * queryParams.PerPage;
            var query = _dbContext.Orders
                .AsNoTracking()
                .Include(data => data.OrderDetails)
                .Where(data => data.DeletedAt == null
                    && data.OrderDetails.All(x => x.DeletedAt == null))
                .AsQueryable();

            query = QuerySearch(query, queryParams);
            query = QueryFilter(query, queryParams);
            query = QuerySort(query, queryParams);

            var data = query.Skip(skip).Take(queryParams.PerPage);
            var count = await CountAsync(data);

            return new PaginationResult<Models.Order>
            {
                Data = await data.ToListAsync(),
                Count = count
            };
        }

        private static IQueryable<Models.Order> QuerySearch(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            if (queryParams.Search != null)
            {
                var searchParam = $"%{queryParams.Search}%";
                query = query.Where(data =>
                    EF.Functions.Like(data.OrderNumber, searchParam) ||
                    data.OrderDetails.Any(x => EF.Functions.Like(x.ExProductCode, searchParam)) ||
                    data.OrderDetails.Any(x => EF.Functions.Like(x.ExProductName, searchParam)));
            }

            return query;
        }

        private static IQueryable<Models.Order> QueryFilter(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            if (queryParams.Status != null)
            {
                List<string> listStatus = [.. queryParams.Status.Split(',')];
                query = query.Where(data => listStatus.Contains(data.Status));
            }

            return query;
        }

        private static IQueryable<Models.Order> QuerySort(IQueryable<Models.Order> query, OrderQueryDto queryParams)
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.Order, object>>> sortFunctions = new()
            {
                { "order_number", data => data.OrderNumber },
                { "created_at", data => data.CreatedAt },
                { "updated_at", data => data.UpdatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.Order, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> CountAsync(IQueryable<Models.Order> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }

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