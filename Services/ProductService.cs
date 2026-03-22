using kanimeclothing.Data;
using kanimeclothing.Models;
using Microsoft.EntityFrameworkCore;

namespace kanimeclothing.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;
        private static List<Product>? _cachedProducts;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            // Check if cache is valid
            if (_cachedProducts != null && DateTime.UtcNow - _cacheTime < CacheDuration)
            {
                _logger.LogInformation("Returning cached products");
                return _cachedProducts;
            }

            // Cache expired or doesn't exist, read from database
            _cachedProducts = await ReadProductsFromDatabaseAsync();
            _cacheTime = DateTime.UtcNow;
            return _cachedProducts;
        }

        private async Task<List<Product>> ReadProductsFromDatabaseAsync()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                _logger.LogInformation($"Successfully loaded {products.Count} products from database");
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading products from database: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var products = await GetProductsAsync();
                return products.FirstOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting product by id: {ex.Message}");
                return null;
            }
        }
    }
}
