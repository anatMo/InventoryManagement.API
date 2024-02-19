using InventoryManagement.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly InventoryManagementDbContext _inventoryManagementDbContext;
        public ProductsController(InventoryManagementDbContext inventoryManagementContext)
        {
            _inventoryManagementDbContext = inventoryManagementContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var employees = await _inventoryManagementDbContext.Products.ToListAsync();

            return Ok(employees);
        }


    }
}
