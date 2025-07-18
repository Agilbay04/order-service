using OrderService.Domain.Product.Dtos;

namespace OrderService.Domain.Order.Dtos
{
    public class OrderResultDto
    {
        public OrderResultDto() { }

        public OrderResultDto(Models.Order order, OrderQueryDto param = null)
        {
            var orderDetails = order.OrderDetails;

            Id = order.Id;
            OrderNumber = order.OrderNumber;
            Customer = new CustomerOrderDto(order);
            Status = order.Status;
            TotalProduct = orderDetails.Count;
            SubTotalPrice = order.SubTotalPrice;
            CreatedAt = order.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            Remark = order.Remark;

            if (param?.WithDetails == true)
            {
                OrderDetails = MapToDetails([.. orderDetails]);
            }
        }

        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public CustomerOrderDto Customer { get; set; }
        public string Status { get; set; }
        public int TotalProduct { get; set; }
        public decimal SubTotalPrice { get; set; }
        public string CreatedAt { get; set; }
        public string Remark { get; set; }
        public List<OrderDetailResultDto> OrderDetails { get; set; }

        public static List<OrderResultDto> MapTo(List<Models.Order> orders, OrderQueryDto param = null)
        {
            return [.. orders.Select(o => new OrderResultDto(o, param))];
        }

        public static List<OrderDetailResultDto> MapToDetails(List<Models.OrderDetail> orderDetails, List<ProductResultDto> products = null)
        {
            return [.. orderDetails.Select(o => new OrderDetailResultDto(o, products))];
        }
    }

    public class CustomerOrderDto
    {
        public CustomerOrderDto() { }

        public CustomerOrderDto(Models.Order order)
        {
            Name = order.CustomerName;
            Address = order.CustomerAddress;
            PhoneNumber = order.CustomerPhoneNumber;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class OrderDetailResultDto
    {
        public OrderDetailResultDto() { }

        public OrderDetailResultDto(Models.OrderDetail orderDetail, List<ProductResultDto> product = null)
        {
            var dataProduct = product?.FirstOrDefault(p => p.Id == orderDetail.ProductId);

            ProductId = orderDetail.ProductId;
            ProductCode = orderDetail.ExProductCode;
            ProductName = orderDetail.ExProductName;
            CategoryName = orderDetail.ExCategoryName;
            UnitPrice = orderDetail.ExUnitPrice;
            Quantity = orderDetail.Quantity;
            TotalPrice = orderDetail.TotalPrice;
            Stock = dataProduct?.Stock;
        }

        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public int? Stock { get; set; } = null;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}