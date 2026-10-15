namespace T_Shirts_API.DtoS.CartDto
{
    public class CartReadDto
    {
        public List<CartItemReadDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
