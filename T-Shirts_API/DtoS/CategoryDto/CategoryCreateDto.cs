using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.CategoryDto
{
    public class CategoryCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
