using Scheduler.API.Models.Payment;

namespace Scheduler.API.Services.Payment
{
    public interface IPayment
    {
        Task<List<PaymentData>> GetPaymentData(string paymentType);
        Task UpdatePaymentStatus(string paymentType, int id, string transactionId);
        Task<PaymentStatus> GetPaymentStatus(string paymentType, int id);
        Task<int> ManualMarkAsPaid(string paymentType, int id, string reason, DateTime? paymentDate);
    }
}
