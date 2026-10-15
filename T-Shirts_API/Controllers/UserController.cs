using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using T_Shirts_API.Context;
using T_Shirts_API.Data;
using T_Shirts_API.DtoS.UserDto;

namespace T_Shirts_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly TShirtAppDBContext _dbContext;

        public UserController(TShirtAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static readonly Expression<Func<User, UserReadDto>> ToReadDto = u =>
        new UserReadDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
        };


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserReadDto>>> GetAllAsync()
        {
            var user = await _dbContext.Users.Select(ToReadDto).ToListAsync();
            return Ok(user);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> GetByIdAsync(int id)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == id)
                .Select(ToReadDto)
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound(new { message = "اليوزر ده مش موجود" });

            return Ok(user);

        }


        [HttpGet("me")]
        public async Task<ActionResult<UserReadDto>> GetMeAsync()
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == CurrentUserId)
                .Select(ToReadDto)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMeAsync(UserUpdateDto dto)
        {
            var user = await _dbContext.Users.FindAsync(CurrentUserId);
            if (user == null)   
                return NotFound();

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;

            await _dbContext.SaveChangesAsync();

            return NoContent();       
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeletMeAync()
        {
            var userId = CurrentUserId;

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            if (await _dbContext.Orders.AnyAsync(o => o.UserId == userId))
                return Conflict(new { message = "مينفعش تمسح حساب له اوردرات" });

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();

                 
        }


    }
}
