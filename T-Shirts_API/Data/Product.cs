namespace T_Shirts_API.Data
{
    public class Product
    {
        // Id, Name, Description, Price, ImageUrl, StockQuantity, IsActive, CategoryId (FK)
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; }  = new List<OrderItem>();
    }
}
