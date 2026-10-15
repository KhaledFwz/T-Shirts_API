using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T_Shirts_API.Context;
using T_Shirts_API.Data;
using T_Shirts_API.DtoS.UserDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace T_Shirts_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TShirtAppDBContext _dbcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;

        public AuthController(TShirtAppDBContext dBContext, IPasswordHasher<User> passwordHasher, IConfiguration config)
        {
            _dbcontext = dBContext;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserReadDto>> Register(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            if (await _dbcontext.Users.AnyAsync(u => u.Email == email))
                return Conflict(new {message = "الايميل ده مسجل قبل كدة" });


            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber.Trim(),
                Role = Role.Customer
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();

            var result = new UserReadDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> LoginAync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Unauthorized(new { message = "الايميل او الباسورد غلط" });

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "الايميل او الباسورد غلط" });

            var (token, expiresAt) = CreateToken(user);
            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                FullName = user.FullName,
                Role = user.Role.ToString()
            });
        }


        private (string Token, DateTime ExpiresAt) CreateToken(User user)
        {
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!));


            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: Claims,
                expires: expiresAt,
                signingCredentials: credential
                );
            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
