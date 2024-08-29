using System.Text.Json;
using RedisExampleApp.API.Models;
using RedisExampleApp.Cache;
using StackExchange.Redis;

namespace RedisExampleApp.API.Repositories
{
    public class ProductRepositoryWithCacheDecorator : IProductRepository
    {
        private const string productKey = "productCaches";
        private readonly IProductRepository _productRepository;
        private readonly RedisService _redisService;
        private readonly IDatabase _cacheRepository;

        public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService)
        {
            _productRepository = productRepository;
            _redisService = redisService;
            _cacheRepository = _redisService.GetDb(2);
        }

        // Decorator Design Pattern - Get ve GetById fonksiyonlari once cache'den veri cekmeye calisir.
        // bir nesneye çalışma zamanında yeni özellikler eklemeyi sağlayan bir yapısal tasarım desenidir.
        // Bu, nesnenin mevcut kodunu değiştirmeden, üzerine yeni katmanlar ekleyerek gerçekleştirilir.
        // Bu sayede nesnenin davranışını esnek bir şekilde genişletebilirsiniz.
        public async Task<List<Product>> GetAsync()
        {
            if (!await _cacheRepository.KeyExistsAsync(productKey))
                return await LoadToCacheFromDbAsync();

            var products = new List<Product>();
            var cacheProducts = await _cacheRepository.HashGetAllAsync(productKey);

            foreach (var item in cacheProducts.ToList())
            {
                var product = JsonSerializer.Deserialize<Product>(item.Value);
                products.Add(product);
            }

            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (_cacheRepository.KeyExists(productKey))
            {
                var product = await _cacheRepository.HashGetAsync(productKey, id);

                return product.HasValue ? JsonSerializer.Deserialize<Product>(product) : null;
            }

            var products = await LoadToCacheFromDbAsync();

            return products.FirstOrDefault(x => x.ProductId == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            var newProduct = await _productRepository.CreateAsync(product);

            if (await _cacheRepository.KeyExistsAsync(productKey))
            {
                await _cacheRepository.HashSetAsync(productKey, newProduct.ProductId,
                    JsonSerializer.Serialize(newProduct));
            }

            return newProduct;
        }

        private async Task<List<Product>> LoadToCacheFromDbAsync()
        {
            var products = await _productRepository.GetAsync();

            products.ForEach(p =>
            {
                _cacheRepository.HashSetAsync(productKey, p.ProductId, JsonSerializer.Serialize(p));
            });

            return products;
        }
    }
}
