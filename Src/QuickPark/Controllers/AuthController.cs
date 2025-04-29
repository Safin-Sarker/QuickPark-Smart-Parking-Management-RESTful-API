using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuickPark.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickPark.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration) 
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("registration")]

        public async Task<IActionResult> Registration(Register request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email,

            };

            var result= await _userManager.CreateAsync(user,request.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered Successfully" });
            }

            return BadRequest(result.Errors);




        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            var currentUser = await _userManager.FindByNameAsync(userLogin.Username);

            if (currentUser == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(currentUser, userLogin.Password);

            if (!isPasswordCorrect)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new[]
             {
                new Claim(JwtRegisteredClaimNames.Sub, currentUser.Id), 
                new Claim(JwtRegisteredClaimNames.UniqueName, currentUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, currentUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwtToken });
        }
    }
}
