namespace gRPC_AspNetCore.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
