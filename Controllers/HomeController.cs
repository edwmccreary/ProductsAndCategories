using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsAndCategories.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.AllProducts = _context.Products.OrderBy(p => p.UpdatedAt);
            return View();
        }

        [HttpPost("add_product")]
        public IActionResult AddProduct(Product newProduct)
        {
            if(ModelState.IsValid)
            {
                _context.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                GetViewbagProducts();
                return View("Index");
            }
        }

        [HttpPost("addToCategory")]
        public IActionResult AddToCategory(Association newAssociation)
        {
            _context.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/product/{newAssociation.ProductId}");
        }

        [HttpGet("product/{id}")]
        public IActionResult ViewProduct(int id)
        {
            Product viewProduct = _context.Products.Include(a => a.Categories).ThenInclude(p => p.Category).FirstOrDefault(c => c.ProductId == id);
            GetViewbagCategories();
            return View(viewProduct);
        }

        [HttpGet("categories")]
        public IActionResult Categories()
        {
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name);
            return View("Categories");
        }

        [HttpPost("add_category")]
        public IActionResult AddCategory(Category newCategory)
        {
            if(ModelState.IsValid)
            {
                _context.Add(newCategory);
                _context.SaveChanges();
                return RedirectToAction("Categories");
            }
            else
            {
                GetViewbagCategories();
                return View("Categories");
            }
        }

        [HttpPost("addToProduct")]
        public IActionResult AddToProduct(Association newAssociation)
        {
            _context.Add(newAssociation);
            _context.SaveChanges();
            return Redirect($"/category/{newAssociation.CategoryId}");
        }

        [HttpGet("category/{id}")]
        public IActionResult ViewCategory(int id)
        {
            Category viewCategory = _context.Categories.Include(c => c.Products).ThenInclude(a => a.Product).FirstOrDefault(p => p.CategoryId == id);
            GetViewbagProducts();
            return View(viewCategory);
        }

        public void GetViewbagProducts()
        {
            ViewBag.AllProducts = _context.Products.OrderBy(p => p.UpdatedAt);
        }

        public void GetViewbagCategories()
        {
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
