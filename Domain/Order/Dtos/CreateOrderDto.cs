using OrderService.Domain.Order.Constants;

namespace OrderService.Domain.Order.Dtos
{
    public class CreateOrderDto
    {
        public CustomerDto Customer { get; set; }
        public List<CreateOrderItemDto> OrderItem { get; set; }
    }

    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Qty { get; set; }
    }

    public class CustomerDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}