using kanimeclothing.Models;
using System.Text.Json;

namespace kanimeclothing.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductService> _logger;
        private static List<Product>? _cachedProducts;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public ProductService(IWebHostEnvironment env, ILogger<ProductService> logger)
        {
            _env = env;
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

            // Cache expired or doesn't exist, read from file
            _cachedProducts = await ReadProductsFromFileAsync();
            _cacheTime = DateTime.UtcNow;
            return _cachedProducts;
        }

        private async Task<List<Product>> ReadProductsFromFileAsync()
        {
            try
            {
                var jsonPath = Path.Combine(_env.WebRootPath, "api", "products.json");
                
                if (!File.Exists(jsonPath))
                {
                    _logger.LogWarning($"Products file not found at {jsonPath}");
                    return new List<Product>();
                }

                var json = await File.ReadAllTextAsync(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                
                using (var doc = JsonDocument.Parse(json))
                {
                    var productsArray = doc.RootElement.GetProperty("products");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsArray.GetRawText(), options);
                    _logger.LogInformation("Successfully loaded products from file");
                    return products ?? new List<Product>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading products: {ex.Message}");
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
