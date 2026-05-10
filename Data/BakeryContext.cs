using Microsoft.EntityFrameworkCore;
using MormorDagnysBageri.Models;

namespace MormorDagnysBageri.Data;

public class BakeryContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<RawMaterial> RawMaterials { get; set; }
    public DbSet<SupplierRawMaterial> SupplierRawMaterials { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SupplierRawMaterial>()
            .HasKey(srm => new { srm.SupplierId, srm.RawMaterialId });

        modelBuilder.Entity<SupplierRawMaterial>()
            .HasOne(srm => srm.Supplier)
            .WithMany(s => s.SupplierRawMaterials)
            .HasForeignKey(srm => srm.SupplierId);

        modelBuilder.Entity<SupplierRawMaterial>()
            .HasOne(srm => srm.RawMaterial)
            .WithMany(rm => rm.SupplierRawMaterials)
            .HasForeignKey(srm => srm.RawMaterialId);

        modelBuilder.Entity<SupplierRawMaterial>()
            .Property(srm => srm.PricePerKg)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<Product>()
            .Property(p => p.PricePerUnit)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.TotalPrice)
            .HasColumnType("decimal(10,2)");

        base.OnModelCreating(modelBuilder);
    }
}
