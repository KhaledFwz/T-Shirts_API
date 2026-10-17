namespace T_Shirts_API.DtoS.OrderDto
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status {  get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<OrderItemReadDto> Items { get; set; } = new();
    }
}
