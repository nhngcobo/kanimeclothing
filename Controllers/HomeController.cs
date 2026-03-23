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
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public HomeController(IProductService productService, ICartService cartService, IPaymentService paymentService, IOrderService orderService)
        {
            _productService = productService;
            _cartService = cartService;
            _paymentService = paymentService;
            _orderService = orderService;
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

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Json(new { success = true });
            }

            return RedirectToAction("ViewCart");
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart(HttpContext.Session);
            
            if (cart.Items.Count == 0)
            {
                return Json(new { success = false, message = "Cart is empty" });
            }

            // Store cart items count for logging
            HttpContext.Session.SetInt32("CartItemCount", cart.Items.Count);
            System.Diagnostics.Debug.WriteLine($"[Checkout] Cart items stored: {cart.Items.Count}");

            return Ok(new { success = true });
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> PaymentCallback()
        {
            var reference = HttpContext.Request.Query["reference"].ToString();
            if (string.IsNullOrWhiteSpace(reference) && HttpContext.Request.HasFormContentType)
            {
                reference = HttpContext.Request.Form["reference"].ToString();
            }

            if (string.IsNullOrWhiteSpace(reference))
            {
                return RedirectToAction("ViewCart");
            }

            // Verify payment with Paystack
            System.Diagnostics.Debug.WriteLine($"[PaymentCallback] Verifying payment with reference: {reference}");
            var isValidPayment = await _paymentService.VerifyPaymentAsync(reference);
            
            System.Diagnostics.Debug.WriteLine($"[PaymentCallback] Verification result: {isValidPayment}");
            
            if (!isValidPayment)
            {
                System.Diagnostics.Debug.WriteLine($"[PaymentCallback] Payment verification failed");
                ViewBag.Reference = reference;
                return View("PaymentCancelled");
            }

            // Payment verified successfully - show success page
            // Stock update will be handled by client-side API call
            System.Diagnostics.Debug.WriteLine($"[PaymentCallback] Payment verified successfully, showing success page");
            ViewBag.Reference = reference;
            return View("PaymentSuccess");
        }

        [HttpPost]
        [Route("api/payment/update-stock")]
        public async Task<IActionResult> UpdateOrderStock([FromBody] UpdateStockRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"[UpdateOrderStock] Received request for {request?.Items?.Count ?? 0} items");
            
            if (request?.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { success = false, message = "No items provided" });
            }

            try
            {
                // Extract product ID and quantity pairs
                var items = request.Items.Select(item => (item.ProductId, item.Quantity)).ToList();
                
                // Update stock
                await _orderService.UpdateStockAsync(items);
                
                // Optionally create order record
                if (!string.IsNullOrWhiteSpace(request.PaystackReference))
                {
                    var cart = new Cart();
                    cart.Items = request.Items.Select(item => new CartItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName ?? "",
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        Color = item.Color,
                        ImageUrl = item.ImageUrl ?? ""
                    }).ToList();
                    
                    var order = await _orderService.CreateOrderAsync(cart, request.PaystackReference, 
                        request.CustomerEmail, request.CustomerPhone);
                    
                    System.Diagnostics.Debug.WriteLine($"[UpdateOrderStock] Order created with ID: {order.Id}");
                }
                
                // Clear cart from session
                _cartService.ClearCart(HttpContext.Session);
                
                System.Diagnostics.Debug.WriteLine($"[UpdateOrderStock] Stock updated successfully");
                return Ok(new { success = true, message = "Stock updated and order created" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateOrderStock] ERROR: {ex.GetType().Name}: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Request models for API endpoints
    public class UpdateStockRequest
    {
        public List<StockItem> Items { get; set; } = new();
        public string? PaystackReference { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
    }

    public class StockItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
    }
}
