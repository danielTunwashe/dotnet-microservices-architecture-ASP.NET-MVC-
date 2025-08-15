using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data;


public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _appDbContext;
    public ProductRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Product?> Create(Product product)
    {
        var NewProduct = await _appDbContext.Products.AddAsync(product);
        await _appDbContext.SaveChangesAsync();
        return NewProduct.Entity;
    }

    public async Task<bool> Delete(Product product)
    {
        try
        {
            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        var allProducts =  await _appDbContext.Products.ToListAsync();
        return allProducts.AsEnumerable();
    }

    public async Task<Product?> GetById(int id)
    {
        var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        return product;
    }

    public async Task<Product?> Update(Product product)
    {
        var productToUpdate = _appDbContext.Update(product);
        await _appDbContext.SaveChangesAsync();

        return productToUpdate.Entity;
    }
}
