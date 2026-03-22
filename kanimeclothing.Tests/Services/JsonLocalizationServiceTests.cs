using kanimeclothing.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace kanimeclothing.Tests.Services
{
    public class JsonLocalizationServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private JsonLocalizationService _service;

        public JsonLocalizationServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null!);
            _service = new JsonLocalizationService(_mockHttpContextAccessor.Object);
        }

        [Fact]
        public void GetString_ReturnsAString()
        {
            // Act
            var result = _service.GetString("Welcome");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        [Fact]
        public void Indexer_ReturnsAString()
        {
            // Act
            var result = _service["Welcome"];

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
    }
}
