namespace ShoeMart.Models.Entities
{
    public class ProductsFormModal
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public string[] Color { get; set; }
    }
}
