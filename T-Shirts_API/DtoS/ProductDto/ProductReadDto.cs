using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.ProductDto
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public string CategoryName { get; set; }
    }
}
