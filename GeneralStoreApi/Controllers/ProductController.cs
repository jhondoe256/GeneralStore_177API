using GeneralStoreApi.Data;
using GeneralStoreApi.Models;
using GeneralStoreApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly GeneralStore177Context _context;

        public ProductController(GeneralStore177Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest req)
        {
            Product newProduct = new()
            {
                Name = req.Name,
                Price = req.Price,
                QuantityInStock = req.Quantity
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return Ok(newProduct);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
    }
}