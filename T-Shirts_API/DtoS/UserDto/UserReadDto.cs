using T_Shirts_API.Data;

namespace T_Shirts_API.DtoS.UserDto
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public Role Role { get; set; }
    }
}
