using InventoryManagement.API.Data;
using InventoryManagement.API.Helpers;
using InventoryManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;

namespace InventoryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public static readonly string SecretKey = "veryverysecret.....";

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
                .FirstOrDefaultAsync(x => x.Email == userRequest.Email);

            if(user == null)
            {
                return NotFound(new {Message = "User Not Found"});
            }

            if (!PasswordHasher.VerifyPassword(userRequest.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userRequest)
        {
            if (userRequest == null) 
            {
                return BadRequest();
            }
            if(await CheckEmailExistAsync(userRequest.Email))
            {
                return BadRequest(new { Message = "Email Already Exist" });
            }

            userRequest.Password = PasswordHasher.HashPassword(userRequest.Password);
            userRequest.Role = "User";
            userRequest.Token = "";

            await _appDbContext.Users.AddAsync(userRequest);
            await _appDbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered"
            });
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _appDbContext.Users.AnyAsync(x => x.Email == email);
        }

        private string CreateJWT(User user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
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
            return Ok(await _appDbContext.Users.ToListAsync());
        }

    }
}
