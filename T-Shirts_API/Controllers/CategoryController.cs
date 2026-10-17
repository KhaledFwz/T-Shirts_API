using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using T_Shirts_API.Context;
using T_Shirts_API.Data;
using T_Shirts_API.DtoS.CategoryDto;

namespace T_Shirts_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly TShirtAppDBContext _dbContext;

        public CategoryController(TShirtAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get All
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryReadDto>>> GetAllAsync()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryReadDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
            return Ok(categories);
        }

        // Get By Id
        [HttpGet("id", Name = "GetCategoryById")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryReadDto>> GetByIdAsync(int id)
        {
            var category = await _dbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryReadDto { Id = c.Id, Name = c.Name })
                .FirstOrDefaultAsync();

            if (category == null)
                return NotFound();


            return Ok(category);
        }


        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateAsync(CategoryCreateDto dto)
        {
            var name = dto.Name.Trim();

            if (await _dbContext.Categories.AnyAsync(c => c.Name == name))
                return Conflict(new { message = "القسم ده موجود بالفعل" });

            var category = new Category { Name = name };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            var result = new CategoryReadDto {Id = category.Id, Name = category.Name};
            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, result);
        }


        [HttpPut("id")]
        public async Task<ActionResult> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var name = dto.Name.Trim();

            if (await _dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != id))
                return Conflict(new { message = "فيه قسم تاني بنفس الاسم" });

            category.Name = name;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("id")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null) return NotFound();

            if (await _dbContext.Products.AnyAsync(p => p.CategoryId == id))
                return Conflict(new { message = "مينفعش تمسح قسم فيه منتجات" });

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
