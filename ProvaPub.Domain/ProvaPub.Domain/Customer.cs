namespace ProvaPub.Domain
{
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<Order> Orders { get; set; }

        public bool HasBoughtBefore()
        {
            return Orders.Any();
        }
    }
}
