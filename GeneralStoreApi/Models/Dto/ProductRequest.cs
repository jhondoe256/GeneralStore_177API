namespace GeneralStoreApi.Models.Dto
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}