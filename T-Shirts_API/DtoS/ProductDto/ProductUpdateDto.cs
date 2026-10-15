using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.ProductDto
{
    public class ProductUpdateDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;
        [Required, MaxLength(1000)]
        public string Description { get; set; } = null!;
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
        [Required, MaxLength(500)]
        public string ImageUrl { get; set; } = null!;
        [Range(0, 2000)]
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}
