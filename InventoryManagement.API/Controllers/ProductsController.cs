using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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
            try
            {
                var products = await _appDbContext.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving products: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product productRequest)
        {
            try
            {
                productRequest.ProductId = Guid.NewGuid();
                await _appDbContext.Products.AddAsync(productRequest);
                await _appDbContext.SaveChangesAsync();
                return Created("Product added", productRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding product: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid productId)
        {
            try
            {
                var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
                if (product == null)
                    return NotFound("Product Not Found");

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving product: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid productId, Product updateProductRequest)
        {
            try
            {
                var product = await _appDbContext.Products.FindAsync(productId);
                if (product == null)
                    return NotFound("Product Not Found");

                product.ProductName = updateProductRequest.ProductName;
                product.Category = updateProductRequest.Category;
                product.Price = updateProductRequest.Price;
                product.UnitsInStock = updateProductRequest.UnitsInStock;

                await _appDbContext.SaveChangesAsync();

                return Ok("Product Updated");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("{productId:Guid}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            try
            {
                var product = await _appDbContext.Products.FindAsync(productId);
                if (product == null)
                    return NotFound();

                _appDbContext.Products.Remove(product);
                await _appDbContext.SaveChangesAsync();

                return Ok("Product Deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting product: " + ex.Message);
            }
        }
    }
}
