namespace Scheduler.API.Models.Payer
{
    /// <summary>Who receives invoices for the client portion: 1=client, 2=org payer, 3=contact (guarantor).</summary>
    public class ClientBillingPreferenceDto
    {
        public Guid ClientId { get; set; }
        public byte BillToType { get; set; } = 1;
        public Guid? PayerId { get; set; }
        public Guid? UserContactId { get; set; }
    }
}
