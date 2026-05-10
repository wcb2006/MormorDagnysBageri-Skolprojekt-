using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;
using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController(BakeryContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var orders = await context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.OrderDate,
                Customer = o.Customer.StoreName,
                OrderItems = o.OrderItems.Select(oi => new
                {
                    oi.Product.ProductName,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.TotalPrice
                })
            })
            .ToListAsync();

        return Ok(new { Success = true, StatusCode = 200, Items = orders.Count, Data = orders });
    }

    [HttpGet("search")]
    public async Task<ActionResult> Search([FromQuery] string? orderNumber, [FromQuery] DateTime? orderDate)
    {
        if (string.IsNullOrWhiteSpace(orderNumber) && orderDate is null)
            return BadRequest("Ange beställningsnummer eller beställningsdatum som sökparameter.");

        var query = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(orderNumber))
            query = query.Where(o => o.OrderNumber.Contains(orderNumber));

        if (orderDate.HasValue)
            query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);

        var orders = await query
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.OrderDate,
                Customer = new
                {
                    o.Customer.Id,
                    o.Customer.StoreName
                },
                OrderItems = o.OrderItems.Select(oi => new
                {
                    oi.Product.ProductName,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.TotalPrice
                })
            })
            .ToListAsync();

        if (!orders.Any())
            return NotFound("Inga beställningar hittades.");

        return Ok(new { Success = true, StatusCode = 200, Items = orders.Count, Data = orders });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var customer = await context.Customers.FindAsync(request.CustomerId);
        if (customer is null)
            return NotFound($"Kund med id {request.CustomerId} hittades inte.");

        var existingOrder = await context.Orders
            .AnyAsync(o => o.OrderNumber == request.OrderNumber);
        if (existingOrder)
            return Conflict($"Beställningsnummer '{request.OrderNumber}' finns redan.");

        var order = new Order
        {
            OrderDate = request.OrderDate,
            OrderNumber = request.OrderNumber,
            CustomerId = request.CustomerId
        };

        foreach (var item in request.Items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            if (product is null)
                return NotFound($"Produkt med id {item.ProductId} hittades inte.");

            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.PricePerUnit,
                TotalPrice = product.PricePerUnit * item.Quantity
            });
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return StatusCode(201, new
        {
            Success = true,
            StatusCode = 201,
            Data = new
            {
                order.Id,
                order.OrderNumber,
                order.OrderDate,
                Customer = customer.StoreName,
                OrderItems = order.OrderItems.Select(oi => new
                {
                    oi.ProductId,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.TotalPrice
                })
            }
        });
    }
}

public record CreateOrderRequest(
    DateTime OrderDate,
    string OrderNumber,
    int CustomerId,
    List<CreateOrderItemRequest> Items);

public record CreateOrderItemRequest(int ProductId, int Quantity);
