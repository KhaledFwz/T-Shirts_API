using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.CartDto
{
    public class UpdateCartItemDto
    {
        [Range(1,100)]
        public int Quantity { get; set; }
    }
}
