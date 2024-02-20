using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public ProductsController(AppDbContext inventoryManagementContext)
        {
            _appDbContext = inventoryManagementContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _appDbContext.Products.ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product productRequest)
        {
            productRequest.ProductId = Guid.NewGuid();

            await _appDbContext.Products.AddAsync(productRequest);
            await _appDbContext.SaveChangesAsync();

            return Ok(productRequest);
        }

        [HttpGet]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid productId)
        {
            var product=
                await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPut]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid productId, Product updateProductRequest)
        {
            var product = await _appDbContext.Products.FindAsync(productId);

            if(product == null)
            {
                return NotFound();
            }

            product.ProductName = updateProductRequest.ProductName;
            product.Category = updateProductRequest.Category;
            product.Price = updateProductRequest.Price;
            product.UnitsInStock = updateProductRequest.UnitsInStock;

            await _appDbContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            var product = await _appDbContext.Products.FindAsync(productId);

            if(product == null)
            {
                return NotFound();
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();

            return Ok(product);
        }
    }
}
