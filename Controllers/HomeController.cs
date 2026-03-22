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
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            var newArrivals = products.Take(3).ToList();
            return View(newArrivals);
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

        [HttpPost]
        public IActionResult PaymentCallback()
        {
            // Handle the return from Paystack after payment
            // Paystack can send either a GET redirect or POST with reference parameter
            
            var reference = HttpContext.Request.Query["reference"].ToString() ?? 
                           HttpContext.Request.Form["reference"].ToString();
            var status = HttpContext.Request.Query["status"].ToString() ?? 
                        HttpContext.Request.Form["status"].ToString();
            
            // Check if payment was successful
            if (status.Equals("Ok", StringComparison.OrdinalIgnoreCase) || 
                !string.IsNullOrEmpty(reference))
            {
                // Payment successful - clear the cart
                _cartService.ClearCart(HttpContext.Session);
                
                // Set ViewBag for the success view
                ViewBag.Reference = reference;
                
                return View("PaymentSuccess");
            }
            
            // Payment was cancelled or failed
            return View("PaymentCancelled");
        }

        [HttpPost]
        [Route("api/payment/verify")]
        public IActionResult VerifyPayment()
        {
            // API endpoint for Paystack to call and verify payment
            // This receives the JSON response {"status":"Ok"}
            
            try
            {
                using (var reader = new System.IO.StreamReader(HttpContext.Request.Body))
                {
                    var body = reader.ReadToEndAsync().Result;
                    
                    // Parse the JSON response
                    var jsonDoc = System.Text.Json.JsonDocument.Parse(body);
                    var root = jsonDoc.RootElement;
                    
                    if (root.TryGetProperty("status", out var statusElement))
                    {
                        var status = statusElement.GetString();
                        
                        if (status?.Equals("Ok", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            // Clear the cart after successful payment
                            _cartService.ClearCart(HttpContext.Session);
                            
                            return Ok(new { success = true, message = "Payment verified and processed successfully" });
                        }
                    }
                }
                
                return BadRequest(new { success = false, message = "Invalid payment status" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
