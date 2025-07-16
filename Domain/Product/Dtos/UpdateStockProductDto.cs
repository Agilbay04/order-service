namespace OrderService.Domain.Product.Dtos
{
    public class UpdateStockProductDto
    {
        public Guid ProductId { get; set; }
        public int Qty { get; set; }
    }
}