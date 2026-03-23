using kanimeclothing.Data;
using kanimeclothing.Models;
using Microsoft.EntityFrameworkCore;

namespace kanimeclothing.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Cart cart, string? paystackReference, string? customerEmail, string? customerPhone);
        Task<Order?> GetOrderByReferenceAsync(string reference);
        Task UpdateStockAsync(List<(int productId, int quantity)> items);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Cart cart, string? paystackReference, string? customerEmail, string? customerPhone)
        {
            var order = new Order
            {
                PaystackReference = paystackReference ?? "",
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                TotalAmount = cart.Total,
                Status = OrderStatus.Completed,
                CreatedDate = DateTime.UtcNow,
                CompletedDate = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            // Add items from cart
            foreach (var cartItem in cart.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    Size = cartItem.Size,
                    Color = cartItem.Color,
                    ImageUrl = cartItem.ImageUrl
                };
                order.Items.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> GetOrderByReferenceAsync(string reference)
        {
            return await _context.Orders
                .Where(o => o.PaystackReference == reference)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateStockAsync(List<(int productId, int quantity)> items)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateStock] Updating stock for {items.Count} items");
            
            foreach (var (productId, quantity) in items)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateStock] Decrementing ProductId={productId}, Qty={quantity}");
                
                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    product.Stock -= quantity;
                    System.Diagnostics.Debug.WriteLine($"[UpdateStock] Updated {product.Name} stock to {product.Stock}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateStock] Product {productId} not found");
                }
            }

            await _context.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"[UpdateStock] Stock update completed");
        }
    }
}
