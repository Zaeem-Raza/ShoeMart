namespace ShoeMart.Models.Entities
{
    public class CartItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public string Image { get; set; }

        public int CurrentCount { get; set; }
    }
}
