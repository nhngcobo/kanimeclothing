using kanimeclothing.Models;
using kanimeclothing.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace kanimeclothing.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_mockEnv.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProductsAsync_WithValidJsonFile_ReturnsProducts()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempDir, "api"));

            var testProducts = new
            {
                products = new[]
                {
                    new { id = 1, name = "Product 1", price = 29.99, category = "Shirt", image = "img1.jpg" },
                    new { id = 2, name = "Product 2", price = 39.99, category = "Pants", image = "img2.jpg" }
                }
            };

            var jsonPath = Path.Combine(tempDir, "api", "products.json");
            var json = JsonSerializer.Serialize(testProducts);
            await File.WriteAllTextAsync(jsonPath, json);

            _mockEnv.Setup(e => e.WebRootPath).Returns(tempDir);

            // Act
            var products = await _service.GetProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count);
            Assert.Equal("Product 1", products[0].Name);
            Assert.Equal(29.99m, products[0].Price);

            // Cleanup
            Directory.Delete(tempDir, true);
        }

        [Fact]
        public async Task GetProductsAsync_WithMissingFile_ReturnsEmptyList()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _mockEnv.Setup(e => e.WebRootPath).Returns(tempDir);

            // Act
            var products = await _service.GetProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Empty(products);
        }

        [Fact]
        public async Task GetProductsAsync_CalledTwiceWithinCacheDuration_ReturnsCachedResult()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempDir, "api"));

            var testProducts = new
            {
                products = new[]
                {
                    new { id = 1, name = "Product 1", price = 29.99, category = "Shirt", image = "img1.jpg" }
                }
            };

            var jsonPath = Path.Combine(tempDir, "api", "products.json");
            var json = JsonSerializer.Serialize(testProducts);
            await File.WriteAllTextAsync(jsonPath, json);

            _mockEnv.Setup(e => e.WebRootPath).Returns(tempDir);

            // Act
            var firstCall = await _service.GetProductsAsync();
            var secondCall = await _service.GetProductsAsync();

            // Assert - Both calls return the same result
            Assert.Equal(firstCall.Count, secondCall.Count);
            Assert.Equal(firstCall[0].Name, secondCall[0].Name);

            // Cleanup
            Directory.Delete(tempDir, true);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempDir, "api"));

            var testProducts = new
            {
                products = new[]
                {
                    new { id = 1, name = "Product 1", price = 29.99, category = "Shirt", image = "img1.jpg" },
                    new { id = 2, name = "Product 2", price = 39.99, category = "Pants", image = "img2.jpg" }
                }
            };

            var jsonPath = Path.Combine(tempDir, "api", "products.json");
            var json = JsonSerializer.Serialize(testProducts);
            await File.WriteAllTextAsync(jsonPath, json);

            _mockEnv.Setup(e => e.WebRootPath).Returns(tempDir);

            // Populate cache
            await _service.GetProductsAsync();

            // Act
            var product = await _service.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
            Assert.Equal("Product 1", product.Name);

            // Cleanup
            Directory.Delete(tempDir, true);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempDir, "api"));

            var testProducts = new
            {
                products = new[]
                {
                    new { id = 1, name = "Product 1", price = 29.99, category = "Shirt", image = "img1.jpg" }
                }
            };

            var jsonPath = Path.Combine(tempDir, "api", "products.json");
            var json = JsonSerializer.Serialize(testProducts);
            await File.WriteAllTextAsync(jsonPath, json);

            _mockEnv.Setup(e => e.WebRootPath).Returns(tempDir);

            // Populate cache
            await _service.GetProductsAsync();

            // Act
            var product = await _service.GetProductByIdAsync(999);

            // Assert
            Assert.Null(product);

            // Cleanup
            Directory.Delete(tempDir, true);
        }
    }
}
