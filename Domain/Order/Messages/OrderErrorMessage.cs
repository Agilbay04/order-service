using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Domain.Order.Messages
{
    public class OrderErrorMessage
    {
        public static string OrderProductOutOfStock(List<string> listProduct) => $"Product {string.Join(",", listProduct)} is out of stock";
    }
}