namespace GeneralStoreApi.Models.Dto
{
    public class TransactionEdit
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }

    }
}