using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Data;

public static class DbSeeder
{
    public static void Seed(BakeryContext context)
    {
        if (context.Suppliers.Any() || context.RawMaterials.Any())
            return;

        var suppliers = new List<Supplier>
        {
            new() { Name = "Nordiska Mjöl AB", Address = "Kvarnvägen 1, 111 22 Stockholm", ContactPerson = "Anna Lindgren", PhoneNumber = "08-123456", Email = "anna@nordiskamjol.se" },
            new() { Name = "Södermalms Socker", Address = "Sockerbruksgatan 5, 118 20 Stockholm", ContactPerson = "Björn Karlsson", PhoneNumber = "08-654321", Email = "bjorn@sodermalmssocker.se" },
            new() { Name = "Göteborgs Ingredienser", Address = "Hamnvägen 12, 411 14 Göteborg", ContactPerson = "Maria Svensson", PhoneNumber = "031-789012", Email = "maria@gbgingredienser.se" }
        };

        context.Suppliers.AddRange(suppliers);
        context.SaveChanges();

        var rawMaterials = new List<RawMaterial>
        {
            new() { ArticleNumber = "RAW-001", Name = "Vetemjöl" },
            new() { ArticleNumber = "RAW-002", Name = "Strösocker" },
            new() { ArticleNumber = "RAW-003", Name = "Smör" },
            new() { ArticleNumber = "RAW-004", Name = "Ägg" },
            new() { ArticleNumber = "RAW-005", Name = "Mjölk" }
        };

        context.RawMaterials.AddRange(rawMaterials);
        context.SaveChanges();

        var links = new List<SupplierRawMaterial>
        {
            new() { SupplierId = suppliers[0].Id, RawMaterialId = rawMaterials[0].Id, PricePerKg = 12.50m },
            new() { SupplierId = suppliers[1].Id, RawMaterialId = rawMaterials[0].Id, PricePerKg = 11.90m },
            new() { SupplierId = suppliers[2].Id, RawMaterialId = rawMaterials[0].Id, PricePerKg = 13.00m },
            new() { SupplierId = suppliers[0].Id, RawMaterialId = rawMaterials[1].Id, PricePerKg = 9.75m },
            new() { SupplierId = suppliers[1].Id, RawMaterialId = rawMaterials[1].Id, PricePerKg = 8.50m },
            new() { SupplierId = suppliers[1].Id, RawMaterialId = rawMaterials[2].Id, PricePerKg = 89.00m },
            new() { SupplierId = suppliers[2].Id, RawMaterialId = rawMaterials[2].Id, PricePerKg = 85.50m },
            new() { SupplierId = suppliers[2].Id, RawMaterialId = rawMaterials[3].Id, PricePerKg = 22.00m },
            new() { SupplierId = suppliers[0].Id, RawMaterialId = rawMaterials[4].Id, PricePerKg = 14.00m },
            new() { SupplierId = suppliers[2].Id, RawMaterialId = rawMaterials[4].Id, PricePerKg = 13.50m },
        };

        context.SupplierRawMaterials.AddRange(links);
        context.SaveChanges();

        var customers = new List<Customer>
        {
            new() { StoreName = "ICA Nära Storgatan", Phone = "08-111222", Email = "bestallning@icastorgatan.se", ContactPerson = "Erik Johansson", DeliveryAddress = "Storgatan 10, 111 23 Stockholm", InvoiceAddress = "Box 100, 111 23 Stockholm" },
            new() { StoreName = "Coop Konsum Söder", Phone = "08-333444", Email = "order@coopsoder.se", ContactPerson = "Lisa Nilsson", DeliveryAddress = "Södra Vägen 5, 118 20 Stockholm", InvoiceAddress = "Södra Vägen 5, 118 20 Stockholm" },
            new() { StoreName = "Hemköp City", Phone = "031-555666", Email = "inkop@hemkopcity.se", ContactPerson = "Karl Andersson", DeliveryAddress = "Avenyn 22, 411 36 Göteborg", InvoiceAddress = "Avenyn 22, 411 36 Göteborg" }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();

        var products = new List<Product>
        {
            new() { ProductName = "Kanelbulle", PricePerUnit = 25.00m, Weight = 0.1, UnitsPerPackage = 6, BestBeforeDate = DateTime.Now.AddDays(5), ManufactureDate = DateTime.Now },
            new() { ProductName = "Semla", PricePerUnit = 35.00m, Weight = 0.15, UnitsPerPackage = 2, BestBeforeDate = DateTime.Now.AddDays(3), ManufactureDate = DateTime.Now },
            new() { ProductName = "Chokladboll", PricePerUnit = 15.00m, Weight = 0.08, UnitsPerPackage = 10, BestBeforeDate = DateTime.Now.AddDays(7), ManufactureDate = DateTime.Now },
            new() { ProductName = "Wienerbröd", PricePerUnit = 30.00m, Weight = 0.12, UnitsPerPackage = 4, BestBeforeDate = DateTime.Now.AddDays(4), ManufactureDate = DateTime.Now }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        var order1 = new Order
        {
            OrderDate = DateTime.Now.AddDays(-2),
            OrderNumber = "ORD-001",
            CustomerId = customers[0].Id,
            OrderItems = new List<OrderItem>
            {
                new() { ProductId = products[0].Id, Quantity = 20, UnitPrice = products[0].PricePerUnit, TotalPrice = products[0].PricePerUnit * 20 },
                new() { ProductId = products[2].Id, Quantity = 10, UnitPrice = products[2].PricePerUnit, TotalPrice = products[2].PricePerUnit * 10 }
            }
        };

        var order2 = new Order
        {
            OrderDate = DateTime.Now.AddDays(-1),
            OrderNumber = "ORD-002",
            CustomerId = customers[1].Id,
            OrderItems = new List<OrderItem>
            {
                new() { ProductId = products[1].Id, Quantity = 30, UnitPrice = products[1].PricePerUnit, TotalPrice = products[1].PricePerUnit * 30 },
                new() { ProductId = products[3].Id, Quantity = 15, UnitPrice = products[3].PricePerUnit, TotalPrice = products[3].PricePerUnit * 15 }
            }
        };

        context.Orders.AddRange(order1, order2);
        context.SaveChanges();
    }
}
