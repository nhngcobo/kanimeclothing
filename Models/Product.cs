namespace kanimeclothing.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ImageName { get; set; }
        public int Stock { get; set; }
    }
}
