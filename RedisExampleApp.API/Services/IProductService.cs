using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAsync(); // ornek disinda kullanimlarda Product nesi yerine DTO nesnesi kullan.

        Task<Product> GetByIdAsync(int ProductId);

        Task<Product> CreateAsync(Product product);
    }
}
