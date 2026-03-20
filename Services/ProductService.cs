using kanimeclothing.Models;
using System.Text.Json;

namespace kanimeclothing.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
    }

    public class ProductService : IProductService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IWebHostEnvironment env, ILogger<ProductService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync()
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
                    return products ?? new List<Product>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading products: {ex.Message}");
                return new List<Product>();
            }
        }
    }
}
