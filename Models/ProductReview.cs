namespace kanimeclothing.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ReviewerName { get; set; }
        public string? ReviewText { get; set; }
        public int Rating { get; set; } // 1-5 stars
        public DateTime CreatedDate { get; set; }
    }
}
