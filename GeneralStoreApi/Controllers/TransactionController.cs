using GeneralStoreApi.Models;
using GeneralStoreApi.Data;
using GeneralStoreApi.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly GeneralStore177Context _context;

        public TransactionController(GeneralStore177Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreate model)
        {
            if (ModelState.IsValid)
            {
                //check if customer exists...
                var customer = await _context.Customers.FindAsync(model.CustomerId);

                if (customer is null)
                    return NotFound();

                //check if product exists
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product is null)
                    return NotFound();

                if (product.QuantityInStock == 0)
                {
                    return BadRequest("Item is out of stock: " + product.QuantityInStock);
                }

                //check if product is in stock
                if (product.QuantityInStock > 0)
                {
                    var availableStock = product.QuantityInStock - model.Quantity;
                    if (availableStock < 0)
                    {
                        return BadRequest("You Ordered too many items. Items left: " + product.QuantityInStock);
                    }
                    else
                    {
                        Transaction transaction = new()
                        {
                            ProductId = model.ProductId,
                            CustomerId = model.CustomerId,
                            Quantity = model.Quantity,
                            DateOfPurchase = DateTime.Now
                        };

                        product.QuantityInStock = availableStock;

                        await _context.Transactions.AddAsync(transaction);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                }
            }

            return BadRequest();

        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _context.Transactions.Include(t => t.Customer).Include(t => t.Product).ToListAsync();

            var transactionListing = transactions.Select(t => new TransactionListItem
            {
                Id = t.Id,
                CustomerName = t.Customer!.Name,
                ProductName = t.Product!.Name,
                Quantity = t.Quantity,
                DateOfPurchase = t.DateOfPurchase
            }).ToList();

            return Ok(transactionListing);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTransaction(TransactionEdit model)
        {
            if (ModelState.IsValid)
            {
                var transaction = await _context.Transactions.FindAsync(model.Id);
                if (transaction is null)
                    return NotFound();
                else
                {
                    //check if customer exists...
                    var customer = await _context.Customers.FindAsync(model.CustomerId);
                    if (customer != null)
                    {
                        transaction.CustomerId = model.CustomerId;
                    }

                    //check if product exists...
                    var product = await _context.Products.FindAsync(model.ProductId);
                    if (product != null)
                    {
                        transaction.ProductId = model.ProductId;
                    }

                    //if model.quantity is greater than the transaction.quantity
                    //then we will have to get the difference between the two
                    //if model.quantity is less than transaction.quantity
                    //then we will have to add the quantity back to the product.quantity field
                    if (model.Quantity > transaction.Quantity)
                    {
                        var difference = model.Quantity - transaction.Quantity;
                        transaction.Quantity = model.Quantity;
                        
                        if(product!.QuantityInStock == 0)
                        {
                            return BadRequest("Item is out of stock.");
                        }
                        
                        if (product.QuantityInStock > 0)
                        {
                            product.QuantityInStock -= difference;
                        }

                    }
                    else if(model.Quantity < transaction.Quantity)
                    {
                        var difference = transaction.Quantity - model.Quantity;
                        transaction.Quantity = model.Quantity;
                        product.QuantityInStock += difference;
                    }

                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{transactionId:int}")]
        public async Task<IActionResult> DeleteTransaction(int transactionId)
        {
            if (transactionId > 0)
            {
                var transaction = await _context.Transactions.FindAsync(transactionId);
                if (transaction is null)
                    return NotFound();
                else
                {
                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();

        }
    }
}