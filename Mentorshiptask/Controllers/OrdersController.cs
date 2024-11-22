//using Mentorshiptask.Context;
using Mentorshiptask.Model;
//using Mentrship_task1.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Mentrship_task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly dbERPContext _context;
        public OrdersController(dbERPContext context)
        {
            _context = context;
        }


        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var product = await _context.TblProducts.FindAsync(orderDto.ProductId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            if (product.NumStock < orderDto.Quantity)
            {
                return BadRequest("Insufficient stock");
            }

            var order = new Order
            {
                ProductId = orderDto.ProductId,
                StrCustomerName = orderDto.CustomerName,
                NumQuantity = orderDto.Quantity,
                DtOrderDate = DateTime.UtcNow
            };

            _context.TblOrders.Add(order);
            product.NumStock -= orderDto.Quantity;

            await _context.SaveChangesAsync();

            return Ok("Orders created successfully");
        }
        [HttpPut("update-order/{orderId}")]
        public async Task<IActionResult> UpdateOrderQuantity(Guid orderId, [FromBody] UpdateOrderDto updateDto)
        {
            var order = await _context.TblOrders.Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            var product = order.Product;
            var quantityDifference = updateDto.Quantity - order.NumQuantity;

            if (product.NumStock < quantityDifference)
            {
                return BadRequest("Insufficient stock");
            }

            order.NumQuantity = updateDto.Quantity;
            product.NumStock -= quantityDifference;

            await _context.SaveChangesAsync();

            return Ok(order);
        }
        [HttpDelete("delete-order/{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _context.TblOrders.Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            var product = order.Product;
            product.NumStock += order.NumQuantity;

            _context.TblOrders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("get-orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.TblOrders.Include(o => o.Product)
                                                 .Select(o => new
                                                 {
                                                     o.Id,
                                                     o.StrCustomerName,
                                                     o.NumQuantity,
                                                     o.DtOrderDate,
                                                     ProductName = o.Product.StrProductName,
                                                     UnitPrice = o.Product.NumUnitPrice
                                                 })
                                                 .ToListAsync();

            return Ok(orders);
        }
        [HttpGet("get-order-summary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            var summary = await _context.TblOrders
                .GroupBy(o => o.Product)
                .Select(g => new
                {
                    ProductName = g.Key.StrProductName,
                    TotalQuantityOrdered = g.Sum(o => o.NumQuantity),
                    TotalRevenue = g.Sum(o => o.NumQuantity * o.Product.NumUnitPrice)
                })
                .ToListAsync();

            return Ok(summary);
        }
        [HttpGet("get-low-stock-products")]
        public async Task<IActionResult> GetLowStockProducts(decimal threshold = 100)
        {
            var products = await _context.TblProducts
                .Where(p => p.NumStock < threshold)
                .Select(p => new
                {
                    p.StrProductName,
                    p.NumUnitPrice,
                    p.NumStock
                })
                .ToListAsync();

            return Ok(products);
        }
        [HttpGet("get-top-customers")]
        public async Task<IActionResult> GetTopCustomers()
        {
            var topCustomers = await _context.TblOrders
                .GroupBy(o => o.StrCustomerName)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalQuantityOrdered = g.Sum(o => o.NumQuantity)
                })
                .OrderByDescending(c => c.TotalQuantityOrdered)
                .Take(3)
                .ToListAsync();

            return Ok(topCustomers);
        }
        [HttpGet("get-products-not-ordered")]
        public async Task<IActionResult> GetProductsNotOrdered()
        {
            var products = await _context.TblProducts
                .Where(p => !_context.TblOrders.Any(o => o.ProductId == p.Id))
                .Select(p => new
                {
                    p.StrProductName,
                    p.NumUnitPrice,
                    p.NumStock
                })
                .ToListAsync();

            return Ok(products);
        }
        [HttpPost("bulk-create-orders")]
        public async Task<IActionResult> BulkCreateOrders([FromBody] List<CreateOrderDto> orderDtos)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var orderDto in orderDtos)
                {
                    var product = await _context.TblProducts.FindAsync(orderDto.ProductId);

                    if (product == null)
                    {
                        return NotFound($"Product {orderDto.ProductId} not found");
                    }

                    if (product.NumStock < orderDto.Quantity)
                    {
                        return BadRequest($"Insufficient stock for product {orderDto.ProductId}");
                    }

                    var order = new Order
                    {
                        ProductId = orderDto.ProductId,
                        StrCustomerName = orderDto.CustomerName,
                        NumQuantity = orderDto.Quantity,
                        DtOrderDate = DateTime.UtcNow
                    };

                    _context.TblOrders.Add(order);
                    product.NumStock -= orderDto.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Orders created successfully");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing the bulk order request.");
            }
        }


    }
}
