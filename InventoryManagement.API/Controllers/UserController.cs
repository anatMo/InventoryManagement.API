using InventoryManagement.API.Data;
using InventoryManagement.API.Helpers;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly string _secretKey = "";

        public UserController(AppDbContext appDbContext, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _secretKey = configuration["AppSettings:SecretKey"];
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userRequest)
        {
            try
            {
                if (userRequest == null)
                {
                    return BadRequest(new { Message = "User is Null" });
                }

                var user = await _appDbContext.Users
                    .FirstOrDefaultAsync(x => x.Email == userRequest.Email);

                if (user == null)
                {
                    return Unauthorized(new { Message = "Email Not Found" });
                }

                if (!PasswordHasher.VerifyPassword(userRequest.Password, user.Password))
                {
                    return Unauthorized(new { Message = "Password is Incorrect" });
                }

                user.Token = CreateJWT(user);
                user.LastLogin = DateTime.Now;
                await _appDbContext.SaveChangesAsync();

                return Ok(new
                {
                    Token = user.Token,
                    Message = "Login Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userRequest)
        {
            try
            {
                if (userRequest == null)
                {
                    return BadRequest(new { Message = "User is Null" });
                }
                if (await CheckEmailExistAsync(userRequest.Email))
                {
                    return BadRequest(new { Message = "Email Already Exist" });
                }

                userRequest.Password = PasswordHasher.HashPassword(userRequest.Password);
                userRequest.Role = "User";
                userRequest.Token = "";

                await _appDbContext.Users.AddAsync(userRequest);
                await _appDbContext.SaveChangesAsync();
                return Ok(new { Message = "User Registered" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Email == email);
        }

        private string CreateJWT(User user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescripted = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtHandler.CreateToken(tokenDescripted);

            return jwtHandler.WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            try
            {
                return Ok(await _appDbContext.Users.ToListAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "User Not Found" });
                }

                _appDbContext.Users.Remove(user);
                await _appDbContext.SaveChangesAsync();

                return Ok(new { Message = "User Deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}