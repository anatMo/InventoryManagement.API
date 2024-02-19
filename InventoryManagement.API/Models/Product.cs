namespace InventoryManagement.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
        public string UnitsInStock { get; set; }
        
    }
}
