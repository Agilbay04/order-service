using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models
{
    public class OrderDetail : BaseModel
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public string ExProductCode { get; set; }

        [MaxLength(150)]
        public string ExProductName { get; set; }

        [Required]
        public string CategoryId { get; set; }

        public string ExCategoryKey { get; set; }

        [MaxLength(50)]
        public string ExCategoryName { get; set; }

        public int Quantity { get; set; }

        public decimal ExUnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}