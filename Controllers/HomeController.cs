using kanimeclothing.Models;
using kanimeclothing.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace kanimeclothing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var cart = _cartService.GetCart(HttpContext.Session);
            ViewData["CartItemCount"] = cart.ItemCount;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Categories()
        {
            return View();
        }

        public async Task<IActionResult> Shop(string? category, string? sort)
        {
            var products = await _productService.GetProductsAsync();
            
            // Filter by category
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                products = products.Where(p => p.Category == category).ToList();
                ViewBag.SelectedCategory = category;
            }
            
            // Sort products
            switch (sort)
            {
                case "low-to-high":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "high-to-low":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "newest":
                    products = products.OrderByDescending(p => p.Id).ToList();
                    break;
                default:
                    products = products.OrderBy(p => p.Id).ToList();
                    break;
            }
            
            ViewBag.SelectedSort = sort;
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            // Get all products to find related items
            var allProducts = await _productService.GetProductsAsync();
            
            // Get 2 related products - prioritize same category
            var relatedProducts = allProducts
                .Where(p => p.Id != id)
                .OrderByDescending(p => p.Category == product.Category)
                .ThenBy(p => p.Id)
                .Take(2)
                .ToList();
            
            // Add sample reviews for each product
            var reviews = new List<ProductReview>
            {
                new ProductReview
                {
                    Id = 1,
                    ProductId = id,
                    ReviewerName = "Sarah M.",
                    ReviewText = "Amazing quality! The product exceeded my expectations. Highly recommend to everyone.",
                    Rating = 5,
                    CreatedDate = DateTime.Now.AddDays(-15)
                },
                new ProductReview
                {
                    Id = 2,
                    ProductId = id,
                    ReviewerName = "John D.",
                    ReviewText = "Great product with excellent service. Will definitely purchase again.",
                    Rating = 4,
                    CreatedDate = DateTime.Now.AddDays(-8)
                }
            };
            
            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.ProductReviews = reviews;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string? size, string? color, int quantity = 1)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }
            
            _cartService.AddToCart(HttpContext.Session, product, size, color, quantity);
            
            return RedirectToAction("ViewCart");
        }

        public IActionResult ViewCart()
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, string? size, string? color)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId, size, color);
            
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, string? size, string? color, int quantity)
        {
            _cartService.UpdateCartQuantity(HttpContext.Session, productId, size, color, quantity);
            return RedirectToAction("ViewCart");
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
