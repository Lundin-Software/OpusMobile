using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Articles;

namespace Opus.Mobile.API.Services.Purchases;

public class PurchaseService(OpusDBContext ctx) : IPurchaseService
{
    public async Task<PurchaseItem> SavePurchase(int userId, SavePurchaseRequest request)
    {
        var purchase = await ctx.Purchases
            .FirstOrDefaultAsync(purchase => purchase.Id == request.PurchaseId);

        if (purchase is null)
        {
            purchase = new Data.Models.Purchases();
            await ctx.Purchases.AddAsync(purchase);
        }

        purchase.EmployeeId = userId;
        purchase.ComponentId = request.ComponentId;
        purchase.UnscheduledTaskId = request.UnschedTaskId;
        purchase.StartTime = request.StartTime;
        purchase.EndTime = request.EndTime;
        purchase.Processed = request.IsProcessed;

        foreach (var item in request.ConsumingArticles)
        {
            var consumingArticle = await ctx.ConsumingArticles
                .FirstOrDefaultAsync(article => article.Id == item.ConsumingArticleId);

            if (consumingArticle is null)
            {
                consumingArticle = new ConsumingArticles();
                await ctx.ConsumingArticles.AddAsync(consumingArticle);
            }

            consumingArticle.Purchase = purchase;
            consumingArticle.ArtId = item.ArticleId;
            consumingArticle.ShelveId = item.ShelveId;
            consumingArticle.UserId = userId;
            consumingArticle.Date = DateTime.Now;
            consumingArticle.Quantity = item.Qty;
        }

        await ctx.SaveChangesAsync();

        return await GetPurchase(purchase.Id);
    }

    private async Task<PurchaseItem> GetPurchase(int purchaseId)
    {
        var purchase = await ctx.Purchases
            .AsNoTracking()
            .Include(purchase => purchase.ConsumingArticles)
                .ThenInclude(article => article.Art)
            .Include(purchase => purchase.ConsumingArticles)
                .ThenInclude(article => article.Shelve)
                    .ThenInclude(shelve => shelve.Rack)
                        .ThenInclude(rack => rack.Stock)
            .FirstAsync(purchase => purchase.Id == purchaseId);

        return new PurchaseItem
        {
            PurchaseId = purchase.Id,
            ComponentId = purchase.ComponentId,
            UnschedTaskId = purchase.UnscheduledTaskId,
            StartTime = purchase.StartTime,
            EndTime = purchase.EndTime,
            IsProcessed = purchase.Processed,
            ConsumingArticles = purchase.ConsumingArticles.Select(article => new ConsumingArticleItem
            {
                ConsumingArticleId = article.Id,
                ArticleId = article.ArtId,
                ArticleNr = article.Art?.Nr,
                ArticleDescription = article.Art?.Description,
                ShelveId = article.ShelveId,
                ShelveName = article.Shelve is null ? null : ConstructShelveName(article.Shelve),
                Qty = article.Quantity,
                QtyDetails = article.Quantity.HasValue
                    ? article.Quantity.Value.ToString()
                    : "0"
            }).ToList()
        };
    }

    private static string ConstructShelveName(Shelves shelve)
    {
        var name = shelve.Name ?? "";

        if (shelve.Rack is not null)
        {
            name += " " + shelve.Rack.Name;

            if (shelve.Rack.Stock is not null)
                name += " " + shelve.Rack.Stock.Name;
        }

        return name;
    }
}
