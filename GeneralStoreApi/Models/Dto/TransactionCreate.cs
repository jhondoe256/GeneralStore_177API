namespace GeneralStoreApi.Models.Dto
{
    public class TransactionCreate
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
    }
}