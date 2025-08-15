namespace Mango.Services.ProductAPI.Models;


public interface IProductRepository
{
    Task<Product?> Create(Product product);
    Task<IEnumerable<Product>> GetAll();
    Task<Product?> GetById(int id);
    Task<Product?> Update(Product product);
    Task<bool> Delete(Product product);
}
