namespace MormorDagnysBageri.Models;

public class Product
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal PricePerUnit { get; set; }
    public double Weight { get; set; }
    public int UnitsPerPackage { get; set; }
    public DateTime BestBeforeDate { get; set; }
    public DateTime ManufactureDate { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
