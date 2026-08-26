using Scheduler.API.Models.Account.Card;
using System.Data;
using Dapper;

namespace Scheduler.API.Services.Account.Card
{
    public class CardRepository : ICard
    {
        IDapperRepository _dapperRepository = null;
        public CardRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }
        public async Task<CardInfo> GetCardInfoAsync(Guid UserId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", UserId, DbType.Guid);
                var result = await Task.FromResult(_dapperRepository.GetList<CardInfo>("[dbo].[GetUserCardInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> UpsertCardInfo(UpsertCardInfoViewModel upsertCardInfoViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pExpiryMonth", upsertCardInfoViewModel.ExpiryMonth, DbType.Int32);
                dp_params.Add("@pExpiryYear", upsertCardInfoViewModel.ExpiryYear, DbType.Int32);
                dp_params.Add("@pTypeId", upsertCardInfoViewModel.TypeId, DbType.Int64);
                dp_params.Add("@pCardNumber", upsertCardInfoViewModel.CardNumber, DbType.String);
                dp_params.Add("@pCardHolderName", upsertCardInfoViewModel.CardHolderName, DbType.String);
                dp_params.Add("@pCVV", upsertCardInfoViewModel.CVV, DbType.String);
                dp_params.Add("@pUserId", upsertCardInfoViewModel.UserId, DbType.Guid);
                dp_params.Add("@pCardId", upsertCardInfoViewModel.CardId, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid>("[dbo].[InserUpdateUserCardInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return upsertCardInfoViewModel.CardId = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
