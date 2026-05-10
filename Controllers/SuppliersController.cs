using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;
using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Controllers;

[Route("api/suppliers")]
[ApiController]
public class SuppliersController(BakeryContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        try
        {
            var suppliers = await context.Suppliers
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Address,
                    s.ContactPerson,
                    s.PhoneNumber,
                    s.Email
                })
                .ToListAsync();

            return Ok(new { Success = true, StatusCode = 200, Items = suppliers.Count, Data = suppliers });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }

    [HttpGet("{id}/products")]
    public async Task<ActionResult> GetSupplierProducts(int id)
    {
        try
        {
            var supplier = await context.Suppliers
                .Include(s => s.SupplierRawMaterials)
                    .ThenInclude(srm => srm.RawMaterial)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier is null)
                return NotFound($"Leverantör med id {id} hittades inte.");

            var result = new
            {
                supplier.Id,
                supplier.Name,
                supplier.Address,
                supplier.ContactPerson,
                supplier.PhoneNumber,
                supplier.Email,
                Products = supplier.SupplierRawMaterials.Select(srm => new
                {
                    srm.RawMaterial.Id,
                    srm.RawMaterial.ArticleNumber,
                    srm.RawMaterial.Name,
                    PricePerKg = srm.PricePerKg
                })
            };

            return Ok(new { Success = true, StatusCode = 200, Items = 1, Data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult> Search([FromQuery] string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Ange ett sökord.");

            var suppliers = await context.Suppliers
                .Include(s => s.SupplierRawMaterials)
                    .ThenInclude(srm => srm.RawMaterial)
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Address,
                    s.ContactPerson,
                    s.PhoneNumber,
                    s.Email,
                    Products = s.SupplierRawMaterials.Select(srm => new
                    {
                        srm.RawMaterial.Id,
                        srm.RawMaterial.ArticleNumber,
                        srm.RawMaterial.Name,
                        PricePerKg = srm.PricePerKg
                    })
                })
                .ToListAsync();

            if (!suppliers.Any())
                return NotFound($"Ingen leverantör med namnet '{name}' hittades.");

            return Ok(new { Success = true, StatusCode = 200, Items = suppliers.Count, Data = suppliers });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }

    [HttpPost("{supplierId}/products")]
    public async Task<ActionResult> AddProduct(int supplierId, [FromBody] AddProductRequest request)
    {
        try
        {
            var supplier = await context.Suppliers.FindAsync(supplierId);
            if (supplier is null)
                return NotFound($"Leverantör med id {supplierId} hittades inte.");

            var rawMaterial = await context.RawMaterials
                .FirstOrDefaultAsync(rm => rm.ArticleNumber == request.ArticleNumber);

            if (rawMaterial is null)
            {
                rawMaterial = new RawMaterial
                {
                    ArticleNumber = request.ArticleNumber,
                    Name = request.Name
                };
                context.RawMaterials.Add(rawMaterial);
                await context.SaveChangesAsync();
            }

            var exists = await context.SupplierRawMaterials
                .AnyAsync(srm => srm.SupplierId == supplierId && srm.RawMaterialId == rawMaterial.Id);

            if (exists)
                return Conflict($"Leverantören har redan produkten '{rawMaterial.Name}'.");

            var link = new SupplierRawMaterial
            {
                SupplierId = supplierId,
                RawMaterialId = rawMaterial.Id,
                PricePerKg = request.PricePerKg
            };

            context.SupplierRawMaterials.Add(link);
            await context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Success = true,
                StatusCode = 201,
                Data = new
                {
                    SupplierId = supplierId,
                    rawMaterial.Id,
                    rawMaterial.ArticleNumber,
                    rawMaterial.Name,
                    request.PricePerKg
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel när vi skulle spara: {ex.Message}");
        }
    }

    [HttpPatch("{supplierId}/products/{rawMaterialId}/price")]
    public async Task<ActionResult> UpdatePrice(int supplierId, int rawMaterialId, [FromBody] UpdatePriceRequest request)
    {
        try
        {
            var link = await context.SupplierRawMaterials
                .FirstOrDefaultAsync(srm => srm.SupplierId == supplierId && srm.RawMaterialId == rawMaterialId);

            if (link is null)
                return NotFound("Produkten hittades inte hos denna leverantör.");

            link.PricePerKg = request.PricePerKg;
            await context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                StatusCode = 200,
                Data = new
                {
                    SupplierId = supplierId,
                    RawMaterialId = rawMaterialId,
                    NewPricePerKg = link.PricePerKg
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }
}

public record AddProductRequest(string ArticleNumber, string Name, decimal PricePerKg);
public record UpdatePriceRequest(decimal PricePerKg);
