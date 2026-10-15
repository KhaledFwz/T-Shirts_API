using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace T_Shirts_API.DtoS.UserDto
{
    public class LoginDto
    {
        [Required,EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
