namespace Scheduler.API.Models.Account.Card
{
    public class UpsertCardInfoViewModel
    {
        public Guid? CardId { get; set; }
        public Guid UserId { get; set; }
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? CVV { get; set; }
        public int TypeId { get; set; }
    }
}
