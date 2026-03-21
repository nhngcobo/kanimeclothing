using kanimeclothing.Models;
using kanimeclothing.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace kanimeclothing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
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
            
            ViewBag.RelatedProducts = relatedProducts;
            return View(product);
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
