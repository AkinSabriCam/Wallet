using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountEntity>(builder =>
        {
            builder.ToTable("accounts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.HasIndex(x => new {x.Currency, x.UserId}).IsUnique();
        });
        
        modelBuilder.Entity<TransactionEntity>(builder =>
        {
            builder.ToTable("accounts");
            builder.HasKey(x => x.Id);
            builder.HasOne<AccountEntity>()
                .WithMany().HasForeignKey(x => x.AccountId);
        });
        
        base.OnModelCreating(modelBuilder);
    }
}