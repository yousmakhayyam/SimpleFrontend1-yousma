using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Warehouse.Domain.Data.Identity;
using Warehouse.Domain.Models.Entities;

namespace Warehouse.Domain.Data;

public class DomainDbContext : IdentityDbContext<ApplicationUser>
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options)
    {

    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Company>()
            .HasOne(c => c.Address)
            .WithOne()
            .HasForeignKey<Company>(c => c.AddressId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Product>()
            .OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PriceAmount");
                money.Property(m => m.Cents).HasColumnName("PriceCents");
                money.Property(m => m.Currency)
                    .HasColumnName("PriceCurrency")
                    .HasConversion<string>()
                    .HasMaxLength(3);
            });

        // Configure DynamicProperties to be stored as JSON
        builder.Entity<Product>()
            .Property(p => p.DynamicProperties)
            .HasConversion(
                new ValueConverter<Dictionary<string, string>, string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)))
            .HasColumnName("DynamicPropertiesJson")
            .HasColumnType("nvarchar(max)"); // Use jsonb if using PostgreSQL, for example. Adjust for your DB type.

        base.OnModelCreating(builder);
    }
}