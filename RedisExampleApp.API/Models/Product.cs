namespace RedisExampleApp.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } // string bir referans tip, nullable olabilir.
        public decimal Price { get; set; }
    }
}
