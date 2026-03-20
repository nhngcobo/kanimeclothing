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
        public string? Url { get; set; }
        public int Stock { get; set; }
        public string? Barcode { get; set; }
        public string? ProductCode { get; set; }
        public string? Material { get; set; }
        public string? Measurements { get; set; }
        public string? Origin { get; set; }
    }
}
