using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Domain.Order.Dtos
{
    public class OrderResultDto(Models.Order order)
    {
        public Guid Id { get; set; } = order.Id;
        public string OrderNumber { get; set; } = order.OrderNumber;
        public CustomerOrderDto Customer { get; set; } = new CustomerOrderDto(order);
        public string Status { get; set; } = order.Status;
        public decimal SubTotalPrice { get; set; } = order.SubTotalPrice;
        public string CreatedAt { get; set; } = order.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss");
        public string Remark { get; set; } = order.Remark;
        public List<OrderDetailResultDto> OrderDetails { get; set; } = MapToDetails(order.OrderDetails.ToList());

        public static List<OrderResultDto> MapTo(List<Models.Order> orders)
        {
            return [.. orders.Select(o => new OrderResultDto(o))];
        }

        private static List<OrderDetailResultDto> MapToDetails(List<Models.OrderDetail> orderDetails)
        {
            return [.. orderDetails.Select(o => new OrderDetailResultDto(o))];
        }
    }

    public class CustomerOrderDto(Models.Order order)
    {
        public string Name { get; set; } = order.CustomerName;
        public string Address { get; set; } = order.CustomerAddress;
        public string PhoneNumber { get; set; } = order.CustomerPhoneNumber;
    }

    public class OrderDetailResultDto(Models.OrderDetail orderDetail)
    {
        public Guid ProductId { get; set; } = orderDetail.ProductId;
        public string ProductCode { get; set; } = orderDetail.ExProductCode;
        public string ProductName { get; set; } = orderDetail.ExProductName;
        public string CategoryName { get; set; } = orderDetail.ExCategoryName;
        public decimal UnitPrice { get; set; } = orderDetail.ExUnitPrice;
        public int Quantity { get; set; } = orderDetail.Quantity;
        public decimal TotalPrice { get; set; } = orderDetail.TotalPrice;
    }
}