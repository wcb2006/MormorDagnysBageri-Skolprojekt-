using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;
using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomersController(BakeryContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var customers = await context.Customers
            .Select(c => new
            {
                c.Id,
                c.StoreName,
                c.Phone,
                c.Email,
                c.ContactPerson,
                c.DeliveryAddress,
                c.InvoiceAddress
            })
            .ToListAsync();

        return Ok(new { Success = true, StatusCode = 200, Items = customers.Count, Data = customers });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var customer = await context.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null)
            return NotFound($"Kund med id {id} hittades inte.");

        var result = new
        {
            customer.Id,
            customer.StoreName,
            customer.Phone,
            customer.Email,
            customer.ContactPerson,
            customer.DeliveryAddress,
            customer.InvoiceAddress,
            Orders = customer.Orders.Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.OrderDate,
                OrderItems = o.OrderItems.Select(oi => new
                {
                    oi.Product.ProductName,
                    oi.Quantity,
                    oi.UnitPrice,
                    oi.TotalPrice
                })
            })
        };

        return Ok(new { Success = true, StatusCode = 200, Items = 1, Data = result });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            StoreName = request.StoreName,
            Phone = request.Phone,
            Email = request.Email,
            ContactPerson = request.ContactPerson,
            DeliveryAddress = request.DeliveryAddress,
            InvoiceAddress = request.InvoiceAddress
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        return StatusCode(201, new
        {
            Success = true,
            StatusCode = 201,
            Data = new
            {
                customer.Id,
                customer.StoreName,
                customer.Phone,
                customer.Email,
                customer.ContactPerson,
                customer.DeliveryAddress,
                customer.InvoiceAddress
            }
        });
    }

}

public record CreateCustomerRequest(
    string StoreName,
    string Phone,
    string Email,
    string ContactPerson,
    string DeliveryAddress,
    string InvoiceAddress);
