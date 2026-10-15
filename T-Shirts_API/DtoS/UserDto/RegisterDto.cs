using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.UserDto
{
    public class RegisterDto
    {
        [Required,MaxLength(100)]
        public string FullName { get; set; } = null!;
        [Required,EmailAddress,MaxLength(150)]
        public string Email { get; set; } = null!;
        [Required,Phone,MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;
        [Required,MinLength(8),MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}
