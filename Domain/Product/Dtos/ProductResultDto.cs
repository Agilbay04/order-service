using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Domain.Product.Dtos
{
    public class ProductResultDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CategoryId { get; set; }
        public string CategoryKey { get; set; }
        public string CategoryName { get; set; }
        public string IsPublish { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string CreatedAt { get; set; }
    }
}