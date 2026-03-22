using kanimeclothing.Models;
using System.Text.Json;

namespace kanimeclothing.Services
{
    public interface ICartService
    {
        Cart GetCart(ISession session);
        void SaveCart(ISession session, Cart cart);
        void AddToCart(ISession session, Product product, string? size, string? color, int quantity = 1);
        void RemoveFromCart(ISession session, int productId, string? size, string? color);
        void UpdateCartQuantity(ISession session, int productId, string? size, string? color, int quantity);
        void ClearCart(ISession session);
    }

    public class CartService : ICartService
    {
        private const string CartSessionKey = "Cart";

        public Cart GetCart(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);
            
            if (string.IsNullOrEmpty(cartJson))
            {
                return new Cart();
            }
            
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Cart>(cartJson, options) ?? new Cart();
        }

        public void SaveCart(ISession session, Cart cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }

        public void AddToCart(ISession session, Product product, string? size, string? color, int quantity = 1)
        {
            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Size = size,
                Color = color,
                Quantity = quantity,
                ImageUrl = product.Url
            };
            
            var cart = GetCart(session);
            
            // Add or update item
            var existing = cart.Items.Find(i =>
                i.ProductId == cartItem.ProductId &&
                i.Size == cartItem.Size &&
                i.Color == cartItem.Color);

            if (existing != null)
                existing.Quantity += cartItem.Quantity;
            else
                cart.Items.Add(cartItem);
            
            SaveCart(session, cart);
        }

        public void RemoveFromCart(ISession session, int productId, string? size, string? color)
        {
            var cart = GetCart(session);
            var existing = cart.Items.Find(i =>
                i.ProductId == productId &&
                i.Size == size &&
                i.Color == color);

            if (existing != null)
            {
                cart.Items.Remove(existing);
            }
            
            SaveCart(session, cart);
        }

        public void UpdateCartQuantity(ISession session, int productId, string? size, string? color, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.Size == size && i.Color == color);
            
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }
            
            SaveCart(session, cart);
        }

        public void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }
    }
}
