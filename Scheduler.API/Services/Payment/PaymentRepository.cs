using Dapper;
using Scheduler.API.Models.Payment;
using System.Data;


namespace Scheduler.API.Services.Payment

{
    public class PaymentRepository : IPayment
    {
        IDapperRepository _dapperRepository = null;
        public PaymentRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public async Task<List<PaymentData>> GetPaymentData(string paymentType)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@PaymentType", paymentType, DbType.String);

            // Use the async method for a single result set
            var paymentData = (await _dapperRepository.GetListAsync<PaymentData>(
                "[payment].[uspGetPaymentData]",
                dp_params,
                commandType: CommandType.StoredProcedure
            )).ToList();

            return paymentData;
        }

        public async Task UpdatePaymentStatus(string paymentType, int id, string transactionId)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pPaymentType", paymentType, DbType.String);
            dp_params.Add("@pId", id, DbType.Int32);
            dp_params.Add("@pTransactionId", transactionId, DbType.String);
            await _dapperRepository.UpdateAsync<bool>("[payment].[uspUpdatePaymentStatus]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<PaymentStatus> GetPaymentStatus(string paymentType, int id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pPaymentType", paymentType, DbType.String);
            dp_params.Add("@pId", id, DbType.Int32);

            var paymentStatus = await _dapperRepository.GetAsync<PaymentStatus>(
                "[payment].[uspGetPaymentStatus]",
                dp_params,
                commandType: CommandType.StoredProcedure
            );

            return paymentStatus;
        }

        public async Task<int> ManualMarkAsPaid(string paymentType, int id, string reason, DateTime? paymentDate)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pPaymentType", paymentType, DbType.String);
            dp_params.Add("@pId", id, DbType.Int32);
            dp_params.Add("@pManualPaymentReason", reason, DbType.String);
            dp_params.Add("@pPaymentDate", paymentDate, DbType.DateTime2);

            var result = await _dapperRepository.GetAsync<int>(
                "[payment].[uspManualMarkPaymentAsPaid]",
                dp_params,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
    }
}
