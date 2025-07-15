using OrderService.Infrastructure.Dtos;

namespace OrderService.Domain.Product.Dtos
{
    public class ProductQueryDto : QueryDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}