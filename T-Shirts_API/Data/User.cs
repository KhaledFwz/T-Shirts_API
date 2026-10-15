namespace T_Shirts_API.Data
{
    public enum Role
    {
        Customer,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; }


        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
