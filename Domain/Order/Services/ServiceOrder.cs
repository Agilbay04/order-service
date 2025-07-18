using Microsoft.EntityFrameworkCore;
using OrderService.Constants.Logger;
using OrderService.Domain.Order.Constants;
using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Order.Messages;
using OrderService.Domain.Order.Repositories;
using OrderService.Domain.Product.Dtos;
using OrderService.Domain.Product.Services;
using OrderService.Infrastructure.Databases;
using OrderService.Infrastructure.Dtos;
using OrderService.Infrastructure.Queues;
using OrderService.Models;
using StackExchange.Redis;

namespace OrderService.Domain.Order.Services
{
    public class ServiceOrder(
        ILoggerFactory loggerFactory,
        DataContext dbContext,
        OrderRepository orderRepository,
        ProductService productService,
        BackgroundTaskQueue _taskQueue,
        IConnectionMultiplexer redis
    ) : IServiceOrder
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ACTIVITY);
        private readonly DataContext _dbContext = dbContext;
        private readonly OrderRepository _orderRepository = orderRepository;
        private readonly ProductService _productService = productService;
        private readonly BackgroundTaskQueue _taskQueue = _taskQueue;
        private readonly IConnectionMultiplexer _redis = redis;

        public async Task<PaginationModel<OrderResultDto>> FindAllAsync(OrderQueryDto param = null)
        {
            var data = await _orderRepository.PaginationAsync(param);
            var formatedData = OrderResultDto.MapTo(data.Data, param);
            var paginate = PaginationModel<OrderResultDto>.Parse(formatedData, data.Count, param);
            return paginate;
        }

        public async Task<PaginationModel<OrderDetailResultDto>> FindOrderDetailAsync(Guid id, ProductQueryDto param = null)
        {
            var listProduct = await _orderRepository.PaginationAsync(id, param);
            var productIds = listProduct?.Data?.Select(o => o.ProductId).ToList();
            var data = await _productService.GetProductByIds(productIds);
            var formatedData = OrderResultDto.MapToDetails(listProduct?.Data, data);
            var paginate = PaginationModel<OrderDetailResultDto>.Parse(formatedData, listProduct.Count, param);
            return paginate;
        }

        public async Task<string> CreateOrderAsync(CreateOrderDto body)
        {
            return await _dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var productIds = body.OrderItem.Select(o => o.ProductId).ToList();
                    var orderNumber = await GenerateOrderNumberRedis();
                    var listProduct = await _productService.GetProductByIds(productIds);
                    var (status, remark) = await CheckStockProductAndGetStatus(orderNumber, body.OrderItem, listProduct);

                    var order = new Models.Order()
                    {
                        OrderNumber = orderNumber,
                        CustomerName = body.Customer.Name,
                        CustomerAddress = body.Customer.Address,
                        CustomerPhoneNumber = body.Customer.PhoneNumber,
                        Status = status,
                        Remark = remark,
                        SubTotalPrice = body.OrderItem.Sum(o => o.Qty * listProduct.Single(p => p.Id == o.ProductId).Price),
                        OrderDetails = [.. body.OrderItem.Select(o => new OrderDetail()
                        {
                            ProductId = o.ProductId,
                            ExProductCode = listProduct.Single(p => p.Id == o.ProductId).Code,
                            ExProductName = listProduct.Single(p => p.Id == o.ProductId).Name,
                            ExUnitPrice = listProduct.Single(p => p.Id == o.ProductId).Price,
                            CategoryId = listProduct.Single(p => p.Id == o.ProductId).CategoryId,
                            ExCategoryKey = listProduct.Single(p => p.Id == o.ProductId).CategoryKey,
                            ExCategoryName = listProduct.Single(p => p.Id == o.ProductId).CategoryName,
                            Quantity = o.Qty,
                            TotalPrice = o.Qty * listProduct.Single(p => p.Id == o.ProductId).Price,
                        })]
                    };

                    await _dbContext.Orders.AddAsync(order);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return orderNumber;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error Create Order: {Message}", ex.Message);
                    throw;
                }
            });
        }

        private async Task<(string, string)> CheckStockProductAndGetStatus(
            string orderNumber,
            List<CreateOrderItemDto> orderItems,
            List<ProductResultDto> listProduct
        )
        {
            string status = OrderStatusConstant.PENDING;
            string remark;

            var listOutOfStock = new List<string>();

            foreach (var orderItem in orderItems)
            {
                var product = listProduct.Single(p => p.Id == orderItem.ProductId);
                if (product.Stock < orderItem.Qty)
                {
                    listOutOfStock.Add(product.Name);
                }
            }

            if (listOutOfStock.Count != 0)
            {
                status = OrderStatusConstant.REJECTED;
                remark = OrderErrorMessage.OrderProductOutOfStock(listOutOfStock);
            }
            else
            {
                status = OrderStatusConstant.CONFIRMED;
                remark = OrderMessage.ProcessOrderToDelivery(orderNumber);

                var paramUpdateStock = orderItems.Select(o => new UpdateStockProductDto()
                {
                    ProductId = o.ProductId,
                    Qty = o.Qty
                }).ToList();

                await QueueUpdateStockProduct(paramUpdateStock);
            }

            return (status, remark);
        }

        private async Task QueueUpdateStockProduct(List<UpdateStockProductDto> param)
        {
            await _taskQueue.QueueBackgroundWorkItemAsync(async ct =>
            {
                await _productService.UpdateStockProductAsync(param);
            });
        }

        private async Task<string> GenerateOrderNumberRedis()
        {
            try
            {
                var today = DateTime.Now;
                var datepart = today.ToString("yyMMdd");
                var prefix = $"ORD{datepart}";

                var redisKey = $"order_seq:{prefix}";

                var db = _redis.GetDatabase();

                long sequence = await db.StringIncrementAsync(redisKey);

                if (sequence == 1)
                {
                    await db.KeyExpireAsync(redisKey, TimeSpan.FromDays(2));
                }

                var newOrderNumber = $"{prefix}{sequence:D7}";

                return newOrderNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Generate Order Number: {Message}", ex.Message);
                throw;
            }
        }
        
    }
}