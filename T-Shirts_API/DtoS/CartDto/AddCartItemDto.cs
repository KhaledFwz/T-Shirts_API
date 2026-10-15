using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.CartDto
{
    public class AddCartItemDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
