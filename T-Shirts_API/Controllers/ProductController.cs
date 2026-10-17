using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using T_Shirts_API.Context;
using T_Shirts_API.Data;
using T_Shirts_API.DtoS.ProductDto;

namespace T_Shirts_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly TShirtAppDBContext _dbContext;

        public ProductController(TShirtAppDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        private static readonly Expression<Func<Product, ProductReadDto>> ToReadDto = p => new ProductReadDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category.Name
        };
        

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductReadDto>>> GetAllAsync([FromQuery] int? categoryId)
        {
            var query =  _dbContext.Products.Where(p => p.IsActive);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var products = await query.Select(ToReadDto).ToListAsync();
            return Ok(products);
        }


        [HttpGet("{id}", Name = "GetProductById")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductReadDto>> GetByIdAsync(int id)
        {
            var product = await _dbContext.Products
                .Where(p => p.Id == id && p.IsActive)
                .Select(ToReadDto)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);

        }
        


        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateAsync (ProductCreateDto dto)
        {
            if (!await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId))
                return BadRequest(new {message = "القسم ده مش موجود"});

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                IsActive = true
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Products
                .Where(p => p.Id == product.Id)
                .Select(ToReadDto)
                .FirstAsync();

            return CreatedAtRoute("GetProductById", new { id = result.Id }, result);
                         
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound(new {message ="المنتج ده مش موجود"});

            if (!await _dbContext.Categories.AnyAsync( c=> c.Id == dto.CategoryId))
                return BadRequest(new {message = "القسم ده مش موجود"});
            product.Name = dto.Name.Trim();
            product.Description = dto.Description.Trim();
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl;
            product.StockQuantity = dto.StockQuantity;
            product.IsActive = dto.IsActive;
            product.CategoryId = dto.CategoryId;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(int id, ProductPatchDto dto)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "المنتج ده مش موجود" });

            if (dto.CategoryId.HasValue)
            {
                if (!await _dbContext.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value))
                    return BadRequest(new { message = "القسم ده مش موجود" });
                product.CategoryId = dto.CategoryId.Value;
            }

            if (dto.Name != null) product.Name = dto.Name.Trim();
            if (dto.Description != null) product.Description = dto.Description.Trim();
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.ImageUrl != null) product.ImageUrl = dto.ImageUrl;
            if (dto.StockQuantity.HasValue) product.StockQuantity = dto.StockQuantity.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            await _dbContext.SaveChangesAsync();
            return NoContent();


        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var product =await _dbContext.Products.FindAsync(id);  
            if (product == null)
                return NotFound(new {messeage = "المنتج ده مش موجود"});

            product.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return NoContent();

        }


    } 

}
