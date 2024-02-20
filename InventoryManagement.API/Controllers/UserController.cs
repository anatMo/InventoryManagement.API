using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public UserController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userRequest)
        {
            if(userRequest == null) 
            {
                return BadRequest();
            }

            var user = await _appDbContext.Users
                .FirstOrDefaultAsync(x => x.Email == userRequest.Email && x.Password == userRequest.Password);
            if(user == null)
            {
                return NotFound(new {Message = "User Not Found"});
            }

            return Ok(new
            {
                Message = "Login Success"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userRequest)
        {
            if(userRequest == null) 
            {
                return BadRequest();
            }

            await _appDbContext.Users.AddAsync(userRequest);
            await _appDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered"
            });
        }

    }
}
