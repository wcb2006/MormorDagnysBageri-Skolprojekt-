using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Data;

namespace MormorDagnysBageri.Controllers;

[Route("api/rawmaterials")]
[ApiController]
public class RawMaterialsController(BakeryContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        try
        {
            var rawMaterials = await context.RawMaterials
                .Include(rm => rm.SupplierRawMaterials)
                    .ThenInclude(srm => srm.Supplier)
                .Select(rm => new
                {
                    rm.Id,
                    rm.ArticleNumber,
                    rm.Name,
                    Suppliers = rm.SupplierRawMaterials.Select(srm => new
                    {
                        srm.Supplier.Id,
                        srm.Supplier.Name,
                        srm.Supplier.ContactPerson,
                        srm.Supplier.Email,
                        srm.Supplier.PhoneNumber,
                        PricePerKg = srm.PricePerKg
                    })
                })
                .ToListAsync();

            return Ok(new { Success = true, StatusCode = 200, Items = rawMaterials.Count, Data = rawMaterials });
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

            var result = await context.RawMaterials
                .Include(rm => rm.SupplierRawMaterials)
                    .ThenInclude(srm => srm.Supplier)
                .Where(rm => rm.Name.ToLower().Contains(name.ToLower()))
                .Select(rm => new
                {
                    rm.Id,
                    rm.ArticleNumber,
                    rm.Name,
                    Suppliers = rm.SupplierRawMaterials.Select(srm => new
                    {
                        srm.Supplier.Id,
                        srm.Supplier.Name,
                        srm.Supplier.ContactPerson,
                        srm.Supplier.Email,
                        srm.Supplier.PhoneNumber,
                        PricePerKg = srm.PricePerKg
                    })
                })
                .ToListAsync();

            if (!result.Any())
                return NotFound($"Ingen råvara med namnet '{name}' hittades.");

            return Ok(new { Success = true, StatusCode = 200, Items = result.Count, Data = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        try
        {
            var rawMaterial = await context.RawMaterials
                .Include(rm => rm.SupplierRawMaterials)
                    .ThenInclude(srm => srm.Supplier)
                .Where(rm => rm.Id == id)
                .Select(rm => new
                {
                    rm.Id,
                    rm.ArticleNumber,
                    rm.Name,
                    Suppliers = rm.SupplierRawMaterials.Select(srm => new
                    {
                        srm.Supplier.Id,
                        srm.Supplier.Name,
                        srm.Supplier.ContactPerson,
                        srm.Supplier.Email,
                        srm.Supplier.PhoneNumber,
                        PricePerKg = srm.PricePerKg
                    })
                })
                .FirstOrDefaultAsync();

            if (rawMaterial is null)
                return NotFound("Hittade ingen råvara.");

            return Ok(new { Success = true, StatusCode = 200, Items = 1, Data = rawMaterial });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Något gick fel: {ex.Message}");
        }
    }
}
