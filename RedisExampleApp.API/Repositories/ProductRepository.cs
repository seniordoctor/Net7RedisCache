using Microsoft.EntityFrameworkCore;
using RedisExampleApp.API.Models;

namespace RedisExampleApp.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get ve GetById fonksiyonlari once cache'den veri cekmeye calisir,
        // eger cache'te veri yoksa db'den veri ceker ve cache'e ekler.
        public async Task<List<Product>> GetAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int ProductId)
        {
            return await _context.Products.FindAsync(ProductId);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
