namespace OrderService.Models
{
    public class Order : BaseModel
    {
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Status { get; set; }
        public decimal SubTotalPrice { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}