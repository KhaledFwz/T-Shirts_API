using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.UserDto
{
    public class UserUpdateDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;
        [Required, Phone,MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;
        
    }
}
