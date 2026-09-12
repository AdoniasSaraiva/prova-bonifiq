namespace ProvaPub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public Customer Customer { get; set; }
    }
}
