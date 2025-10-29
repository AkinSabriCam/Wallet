using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShoppingBasketRepository : IShoppingBasketRepository
{
    private readonly ShoppingDbContext _context;
    private readonly DbSet<ShoppingBasketEntity> shoppingBasket;
    
    public ShoppingBasketRepository(ShoppingDbContext context)
    {
        _context = context;
        shoppingBasket = context.Set<ShoppingBasketEntity>();
    }

    public async Task<ShoppingBasketEntity> GetShoppingBasket(Guid shoppingBasketId)
    {
        return await shoppingBasket.FirstOrDefaultAsync(x=>x.Id == shoppingBasketId);
    }

    public async Task<ShoppingBasketEntity> Add(ShoppingBasketEntity shoppingBasketEntity)
    {
        await shoppingBasket.AddAsync(shoppingBasketEntity);

        await _context.SaveChangesAsync();
        return shoppingBasketEntity;
    }
}