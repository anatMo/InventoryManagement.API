namespace InventoryManagement.API.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public long Price { get; set; }
        public long UnitsInStock { get; set; }
        
    }
}
