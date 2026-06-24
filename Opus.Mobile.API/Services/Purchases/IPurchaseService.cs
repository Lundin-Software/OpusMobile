using Opus.Mobile.Shared.Articles;

namespace Opus.Mobile.API.Services.Purchases;

public interface IPurchaseService
{
    Task<PurchaseItem> SavePurchase(int userId, SavePurchaseRequest request);
}
