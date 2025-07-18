using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Order.Constants;
using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Product.Dtos;
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

        private static IQueryable<Models.Order> QuerySearch(
            IQueryable<Models.Order> query,
            OrderQueryDto queryParams
        )
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

        private static IQueryable<Models.Order> QueryFilter(
            IQueryable<Models.Order> query,
            OrderQueryDto queryParams
        )
        {
            if (queryParams.Id != Guid.Empty && queryParams.Id != null)
            {
                query = query.Where(data => data.Id == queryParams.Id);
            }
            
            if (queryParams.Status != null)
            {
                List<string> listStatus = [.. queryParams.Status.Split(',')];
                query = query.Where(data => listStatus.Contains(data.Status));
            }

            return query;
        }

        private static IQueryable<Models.Order> QuerySort(
            IQueryable<Models.Order> query,
            OrderQueryDto queryParams
        )
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

        public async Task<PaginationResult<Models.OrderDetail>> PaginationAsync(
            Guid orderId,
            ProductQueryDto param = null
        )
        {
            int skip = (param.Page - 1) * param.PerPage;
            var query = _dbContext.OrderDetails
                .AsNoTracking()
                .Where(o => o.OrderId == orderId
                    && o.DeletedAt == null)
                .AsQueryable();

            query = QuerySearch(query, param);
            query = QueryFilter(query, param);
            query = QuerySort(query, param);

            var data = query.Skip(skip).Take(param.PerPage);
            var count = await CountAsync(data);

            return new PaginationResult<Models.OrderDetail>
            {
                Data = await data.ToListAsync(),
                Count = count
            };
        }

        private static IQueryable<Models.OrderDetail> QuerySearch(
            IQueryable<Models.OrderDetail> query,
            ProductQueryDto queryParams
        )
        {
            if (queryParams.Search != null)
            {
                var searchParam = $"%{queryParams.Search}%";
                query = query.Where(data =>
                    EF.Functions.Like(data.ExProductCode, searchParam) ||
                    EF.Functions.Like(data.ExProductName, searchParam) ||
                    EF.Functions.Like(data.ExCategoryName, searchParam));
            }

            return query;
        }

        private static IQueryable<Models.OrderDetail> QueryFilter(
            IQueryable<Models.OrderDetail> query,
            ProductQueryDto queryParams
        )
        {
            if (queryParams.Code != null)
            {
                query = query.Where(data => data.ExProductCode == queryParams.Code);
            }

            return query;
        }

        private static IQueryable<Models.OrderDetail> QuerySort(
            IQueryable<Models.OrderDetail> query,
            ProductQueryDto queryParams
        )
        {
            queryParams.SortBy ??= "updated_at";

            Dictionary<string, Expression<Func<Models.OrderDetail, object>>> sortFunctions = new()
            {
                { "product_code", data => data.ExProductCode },
                { "created_at", data => data.CreatedAt },
                { "updated_at", data => data.UpdatedAt },
            };

            if (!sortFunctions.TryGetValue(queryParams.SortBy, out Expression<Func<Models.OrderDetail, object>> value))
            {
                throw new BadHttpRequestException($"Invalid sort column: {queryParams.SortBy}, available sort columns: " + string.Join(", ", sortFunctions.Keys));
            }

            query = queryParams.Order == SortOrder.Asc
                ? query.OrderBy(value).AsQueryable()
                : query.OrderByDescending(value).AsQueryable();

            return query;
        }

        public async Task<int> CountAsync(IQueryable<Models.OrderDetail> query)
        {
            return await query.Select(x => x.Id).CountAsync();
        }
        
    }
}