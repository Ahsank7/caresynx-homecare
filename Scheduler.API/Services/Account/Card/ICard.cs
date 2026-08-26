using Scheduler.API.Models.Account.Card;

namespace Scheduler.API.Services.Account.Card
{
    public interface ICard
    {
        Task<CardInfo> GetCardInfoAsync(Guid UserId);
        Task<Guid?> UpsertCardInfo(UpsertCardInfoViewModel upsertCardInfoViewModel);

    }
}
