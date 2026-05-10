namespace MormorDagnysBageri.Models;

public class RawMaterial
{
    public int Id { get; set; }
    public string ArticleNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<SupplierRawMaterial> SupplierRawMaterials { get; set; } = [];
}
