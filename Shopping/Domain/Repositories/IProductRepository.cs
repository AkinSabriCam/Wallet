using Domain.Entities;

namespace Domain.Repositories;

public interface IProductRepository
{
    Task<ProductEntity> GetProduct(Guid productId);
    
    Task<List<ProductEntity>> GetProducts();
    
    Task AddProduct(ProductEntity productEntity);
}