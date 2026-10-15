namespace T_Shirts_API.DtoS.UserDto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
