namespace T_Shirts_API.DtoS.OrderDto
{
    public class OrderItemReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

    }
}
