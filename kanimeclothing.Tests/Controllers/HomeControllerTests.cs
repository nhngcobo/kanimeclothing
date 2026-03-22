using kanimeclothing.Controllers;
using kanimeclothing.Models;
using kanimeclothing.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace kanimeclothing.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ICartService> _mockCartService;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockCartService = new Mock<ICartService>();
            _mockPaymentService = new Mock<IPaymentService>();
            _mockOrderService = new Mock<IOrderService>();
            _controller = new HomeController(_mockProductService.Object, _mockCartService.Object, _mockPaymentService.Object, _mockOrderService.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Categories_ReturnsViewResult()
        {
            // Act
            var result = _controller.Categories();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Shop_WithoutFilters_ReturnsAllProducts()
        {
            // Arrange
            var testProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 29.99m, Category = "Shirt" },
                new Product { Id = 2, Name = "Product 2", Price = 39.99m, Category = "Pants" }
            };

            _mockProductService.Setup(s => s.GetProductsAsync())
                .ReturnsAsync(testProducts);

            // Act
            var result = await _controller.Shop(null, null);

            // Assert
            Assert.IsType<ViewResult>(result);
            _mockProductService.Verify(s => s.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task Shop_WithCategoryFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var testProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 29.99m, Category = "Shirt" },
                new Product { Id = 2, Name = "Product 2", Price = 39.99m, Category = "Pants" }
            };

            _mockProductService.Setup(s => s.GetProductsAsync())
                .ReturnsAsync(testProducts);

            // Act
            var result = await _controller.Shop("Shirt", null);

            // Assert
            Assert.IsType<ViewResult>(result);
            _mockProductService.Verify(s => s.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task Details_WithValidProductId_ReturnsProduct()
        {
            // Arrange
            var product = new Product 
            { 
                Id = 1, 
                Name = "Test Product", 
                Price = 29.99m, 
                Category = "Shirt",
                Url = "test.jpg"
            };

            _mockProductService.Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsType<ViewResult>(result);
            _mockProductService.Verify(s => s.GetProductByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task Details_WithInvalidProductId_ReturnsNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
