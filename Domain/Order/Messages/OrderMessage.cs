namespace OrderService.Domain.Order.Messages
{
    public class OrderMessage
    {
        public static string SuccessCreateOrder(string orderNumber) => $"Success create order {orderNumber}";
        public static string ProcessOrderToDelivery(string orderNumber) => $"Process to delivery order {orderNumber}";
    }
}