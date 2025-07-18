using OrderService.Infrastructure.Dtos;

namespace OrderService.Domain.Order.Dtos
{
    public class OrderQueryDto : QueryDto
    {
        public Guid? Id { get; set; }
        public string Status { get; set; }
        public bool WithDetails { get; set; } = false;
    }
}