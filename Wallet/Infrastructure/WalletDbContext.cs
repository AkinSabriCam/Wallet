using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>(builder =>
        {
            // Account entity configuration
            builder.ToTable("accounts");
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Version)
                .HasColumnName("xmin")       
                .HasColumnType("xid")
                .IsRowVersion()
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
            
            builder.HasIndex(x => new { x.Currency, x.UserId }).IsUnique();
        });
        
        modelBuilder.Entity<TransactionEntity>(builder =>
        {
            builder.ToTable("transactions");
            builder.HasKey(x => x.Id);
            builder.HasOne<AccountEntity>()
                .WithMany().HasForeignKey(x => x.AccountId);
        });
        
        modelBuilder.Entity<HttpRequestEntity>(builder =>
        {
            builder.ToTable("http_requests");
            builder.HasKey(x => new {x.UserId, x.Path, x.RequestId, x.BodyHash});
            builder.Property(x => x.Status).HasConversion<string>();
        });
        
        base.OnModelCreating(modelBuilder);
    }
}