using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ShoppingDbContext _context;
    private readonly DbSet<ProductEntity> _products;

    public ProductRepository(ShoppingDbContext context)
    {
        _context = context;
        _products = context.Set<ProductEntity>();
    }

    public async Task<ProductEntity> GetProduct(Guid productId)
    {
        return await _products.FirstOrDefaultAsync(x=>x.Id == productId);
    }

    public async Task<List<ProductEntity>> GetProducts()
    {
        return await _products.ToListAsync();
    }

    public async Task AddProduct(ProductEntity productEntity)
    {
        await _products.AddAsync(productEntity);

        await _context.SaveChangesAsync();
    }
}