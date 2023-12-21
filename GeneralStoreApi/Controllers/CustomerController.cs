using GeneralStoreApi.Data;
using GeneralStoreApi.Models;
using GeneralStoreApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
         private readonly GeneralStore177Context _context;

        public CustomerController(GeneralStore177Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromForm] CustomerEdit newCustomer)
        {
            Customer customer = new()
            {
                Name=newCustomer.Name,
                Email = newCustomer.Email
            };

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }


    }
}