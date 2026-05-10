namespace MormorDagnysBageri.Models;

public class SupplierRawMaterial
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int RawMaterialId { get; set; }
    public RawMaterial RawMaterial { get; set; } = null!;

    public decimal PricePerKg { get; set; }
}
