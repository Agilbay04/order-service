namespace OrderService.Domain.Order.Dtos
{
    public class OrderResultDto
    {
        public OrderResultDto() { }

        public OrderResultDto(Models.Order order)
        {
            Id = order.Id;
            OrderNumber = order.OrderNumber;
            Customer = new CustomerOrderDto(order);
            Status = order.Status;
            SubTotalPrice = order.SubTotalPrice;
            CreatedAt = order.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
            Remark = order.Remark;
            OrderDetails = MapToDetails(order.OrderDetails.ToList());
        }

        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public CustomerOrderDto Customer { get; set; }
        public string Status { get; set; }
        public decimal SubTotalPrice { get; set; }
        public string CreatedAt { get; set; }
        public string Remark { get; set; }
        public List<OrderDetailResultDto> OrderDetails { get; set; }

        public static List<OrderResultDto> MapTo(List<Models.Order> orders)
        {
            return orders.Select(o => new OrderResultDto(o)).ToList();
        }

        private static List<OrderDetailResultDto> MapToDetails(List<Models.OrderDetail> orderDetails)
        {
            return orderDetails.Select(o => new OrderDetailResultDto(o)).ToList();
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

        public OrderDetailResultDto(Models.OrderDetail orderDetail)
        {
            ProductId = orderDetail.ProductId;
            ProductCode = orderDetail.ExProductCode;
            ProductName = orderDetail.ExProductName;
            CategoryName = orderDetail.ExCategoryName;
            UnitPrice = orderDetail.ExUnitPrice;
            Quantity = orderDetail.Quantity;
            TotalPrice = orderDetail.TotalPrice;
        }

        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}