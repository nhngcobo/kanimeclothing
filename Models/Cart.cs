namespace kanimeclothing.Models
{
    public class Cart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();      
        public decimal Total => Items.Sum(i => i.Total);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }
}
