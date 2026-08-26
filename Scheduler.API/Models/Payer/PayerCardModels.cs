namespace Scheduler.API.Models.Payer
{
    /// <summary>Masked payer card on file (organization payer auto-charge).</summary>
    public class PayerCardInfoDto
    {
        public Guid? CardId { get; set; }
        public Guid PayerId { get; set; }
        public string CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? CVV { get; set; }
        public int TypeId { get; set; }
    }

    public class UpsertPayerCardViewModel
    {
        public Guid OrganizationId { get; set; }
        public Guid PayerId { get; set; }
        public Guid? CardId { get; set; }
        public string CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? CVV { get; set; }
        public int TypeId { get; set; }
    }
}
