using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml;
using T_Shirts_API.Context;
using T_Shirts_API.DtoS.CartDto;

namespace T_Shirts_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly TShirtAppDBContext _dbcontext;

        public CartController(TShirtAppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task<CartReadDto> BuiltCartDtoAsync(int UserId)
        {
            var items = await _dbcontext.CartItems
                .Where(ci => ci.Cart.UserId == UserId)
                .Select(ci => new CartItemReadDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ImageUrl = ci.Product.ImageUrl,
                    UnitPrice = ci.Quantity,
                    LineTotal = ci.Product.Price * ci.Quantity
                }).ToListAsync();

            return new CartReadDto
            {
                Items = items,
                TotalAmount = items.Sum(i => i.LineTotal)

            };

        }

        [HttpGet]
        public async Task<ActionResult<CartReadDto>> GetMyCartAsync()
        {
            return Ok(await BuiltCartDtoAsync(CurrentUserId));
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartReadDto>> AddItemsAsync(AddCartItemDto dto)
        {
            var UserId = CurrentUserId;
            var product = await _dbcontext.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
            if (product == null)
                return NotFound(new { message = "المنتج ده مش موجود" });

            var cart = await _dbcontext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (cart == null)
            {
                cart = new Data.Cart { UserId = UserId };
                _dbcontext.Carts.Add(cart);
            }

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            var newQuantity = (item?.Quantity ?? 0) + dto.Quantity;

            if (newQuantity > product.StockQuantity)
                return BadRequest(new { message = $"المتاح من المنتج ده{product.StockQuantity} بس " });

            if (item == null)
                cart.CartItems.Add(new Data.CartItem { ProductId = product.Id, Quantity = dto.Quantity });
            else
                item.Quantity = newQuantity;


            await _dbcontext.SaveChangesAsync();

            return Ok(await BuiltCartDtoAsync(UserId));


        }

        [HttpPut("items/{items:int}")]
        public async Task<ActionResult<CartReadDto>> UpdateItemAsync(int itemId,UpdateCartItemDto dto)
        {
            var userId = CurrentUserId;

            var item = await _dbcontext.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync();
            if (item == null)
                return NotFound(new { message = "العنصر ده مش في عربيتك" });

            if (dto.Quantity > item.Product.StockQuantity)
                return BadRequest(new { message = $"المتاح من المنتج ده{item.Product.StockQuantity}بس" });

            item.Quantity = dto.Quantity;
            await _dbcontext.SaveChangesAsync();
            return Ok(await BuiltCartDtoAsync(userId));
        }


        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItemAsync(int itemId)
        {
            var item = await _dbcontext.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart.UserId == CurrentUserId);

            if (item == null)
                return NotFound(new { message = "العنصر ده مش ف عربيتك" });

            _dbcontext.CartItems.Remove(item);
            await _dbcontext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAsync()
        {
            var items = await _dbcontext.CartItems
                .Where(ci => ci.Cart.UserId == CurrentUserId)
                .ToListAsync();
            _dbcontext.CartItems.RemoveRange(items);
            await _dbcontext.SaveChangesAsync();    
            return NoContent(); 
        }
    }
}
