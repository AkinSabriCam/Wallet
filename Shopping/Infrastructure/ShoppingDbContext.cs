using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

/// <summary>
/// Shopping Db Context
/// </summary>
public class ShoppingDbContext : DbContext
{
    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>(builder =>
        {
            builder.ToTable("products");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Price).IsRequired();
            
            builder.HasMany(x=>x.Baskets)
                .WithOne().HasForeignKey(y=>y.ProductId);
        });
        
        modelBuilder.Entity<ShoppingBasketEntity>(builder =>
        {
            builder.ToTable("shopping_baskets");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalAmount).IsRequired();
            
            builder.HasMany(x=>x.Products)
                .WithOne().HasForeignKey(y=>y.ShoppingBasketId);
        });
        
        modelBuilder.Entity<ShoppingBasketProductEntity>(builder =>
        {
            builder.ToTable("shopping_basket_products");
            builder.HasKey(x => new { x.ShoppingBasketId, x.ProductId });
        });
        
        base.OnModelCreating(modelBuilder);
    }
    
    
}