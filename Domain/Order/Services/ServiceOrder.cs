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

namespace OrderService.Domain.Order.Services
{
    public class ServiceOrder(
        ILoggerFactory loggerFactory,
        DataContext dbContext,
        OrderRepository orderRepository,
        ProductService productService,
        IServiceProvider serviceProvider,
        BackgroundTaskQueue _taskQueue
    ) : IServiceOrder
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ACTIVITY);
        private readonly DataContext _dbContext = dbContext;
        private readonly OrderRepository _orderRepository = orderRepository;
        private readonly ProductService _productService = productService;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly BackgroundTaskQueue _taskQueue = _taskQueue;

        public async Task<PaginationModel<OrderResultDto>> FindAllAsync(OrderQueryDto param = null)
        {
            var data = await _orderRepository.PaginationAsync(param);
            var formatedData = OrderResultDto.MapTo(data.Data);
            var paginate = PaginationModel<OrderResultDto>.Parse(formatedData, data.Count, param);
            return paginate;
        }

        public async Task<string> CreateOrder(CreateOrderDto body)
        {
            return await _dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var productIds = body.OrderItem.Select(o => o.ProductId).ToList();
                    var orderNumber = await GenerateOrderNumber();
                    var listProduct = await _productService.GetProductByIds(productIds);

                    var order = new Models.Order()
                    {
                        OrderNumber = orderNumber,
                        CustomerName = body.Customer.Name,
                        CustomerAddress = body.Customer.Address,
                        CustomerPhoneNumber = body.Customer.PhoneNumber,
                        Status = OrderStatusConstant.PENDING,
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

                    // Check stock product
                    await _taskQueue.QueueBackgroundWorkItemAsync(async ct =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var scopedDbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                        var scopedProductService = scope.ServiceProvider.GetRequiredService<ProductService>();
                        await CheckStockProduct(
                            order.Id,
                            body.OrderItem,
                            listProduct,
                            scopedDbContext,
                            scopedProductService
                        );
                    });

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

        private async Task CheckStockProduct(
            Guid OrderId,
            List<CreateOrderItemDto> orderItems,
            List<ProductResultDto> listProduct,
            DataContext dbContext = null,
            ProductService productService = null
        )
        {
            await dbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();

                try
                {
                    var listOutOfStock = new List<string>();

                    var order = await dbContext.Orders
                        .Where(o => o.Id == OrderId
                            && o.DeletedAt == null
                            && o.Status == OrderStatusConstant.PENDING)
                        .FirstOrDefaultAsync();

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
                        order.Status = OrderStatusConstant.REJECTED;
                        order.Remark = OrderErrorMessage.OrderProductOutOfStock(listOutOfStock);
                    }
                    else
                    {
                        order.Status = OrderStatusConstant.CONFIRMED;
                        order.Remark = OrderMessage.ProcessOrderToDelivery(order.OrderNumber);

                        var paramUpdateStock = orderItems.Select(o => new UpdateStockProductDto()
                        {
                            ProductId = o.ProductId,
                            Qty = o.Qty
                        }).ToList();
                        
                        await productService.UpdateStockProductAsync(paramUpdateStock);
                    }

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error Check Stock Product: {Message}", ex.Message);
                    throw;
                }
            });
        }

        private async Task<string> GenerateOrderNumber()
        {
            try
            {
                var today = DateTime.Now;
                var datepart = today.ToString("yyMMdd");
                var prefix = $"ORD{datepart}";

                var lastOrder = await _orderRepository.GetLatestOrderNumber(prefix);

                int lastSequence = 0;
                if (lastOrder != null)
                {
                    var lasSeqString = lastOrder[prefix.Length..];
                    lastSequence = int.TryParse(lasSeqString, out int seq) ? seq : 0;
                }

                var newSequence = lastSequence + 1;
                var newOrderNumber = $"{prefix}{newSequence:D7}";

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