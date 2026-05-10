using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;
using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(BakeryContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var products = await context.Products
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.PricePerUnit,
                p.Weight,
                p.UnitsPerPackage,
                p.BestBeforeDate,
                p.ManufactureDate
            })
            .ToListAsync();

        return Ok(new { Success = true, StatusCode = 200, Items = products.Count, Data = products });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var product = await context.Products
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.PricePerUnit,
                p.Weight,
                p.UnitsPerPackage,
                p.BestBeforeDate,
                p.ManufactureDate
            })
            .FirstOrDefaultAsync();

        if (product is null)
            return NotFound($"Produkt med id {id} hittades inte.");

        return Ok(new { Success = true, StatusCode = 200, Items = 1, Data = product });
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            ProductName = request.ProductName,
            PricePerUnit = request.PricePerUnit,
            Weight = request.Weight,
            UnitsPerPackage = request.UnitsPerPackage,
            BestBeforeDate = request.BestBeforeDate,
            ManufactureDate = request.ManufactureDate
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        return StatusCode(201, new
        {
            Success = true,
            StatusCode = 201,
            Data = new
            {
                product.Id,
                product.ProductName,
                product.PricePerUnit,
                product.Weight,
                product.UnitsPerPackage,
                product.BestBeforeDate,
                product.ManufactureDate
            }
        });
    }

    [HttpPatch("{id}/price")]
    public async Task<ActionResult> UpdatePrice(int id, [FromBody] UpdateProductPriceRequest request)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
            return NotFound($"Produkt med id {id} hittades inte.");

        product.PricePerUnit = request.PricePerUnit;
        await context.SaveChangesAsync();

        return Ok(new
        {
            Success = true,
            StatusCode = 200,
            Data = new
            {
                product.Id,
                product.ProductName,
                NewPricePerUnit = product.PricePerUnit
            }
        });
    }
}

public record CreateProductRequest(
    string ProductName,
    decimal PricePerUnit,
    double Weight,
    int UnitsPerPackage,
    DateTime BestBeforeDate,
    DateTime ManufactureDate);

public record UpdateProductPriceRequest(decimal PricePerUnit);
