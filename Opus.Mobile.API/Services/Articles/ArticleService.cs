using Microsoft.EntityFrameworkCore;
using Opus.Mobile.Data.Context;
using Opus.Mobile.Data.Models;
using Opus.Mobile.Shared.Articles;

namespace Opus.Mobile.API.Services.Articles;

public class ArticleService(OpusDBContext ctx) : IArticleService
{
    public async Task<ArticleItem?> GetArticleByBarcode(string barcode)
    {
        var article = await ctx.ArticlesBarCodes
            .AsNoTracking()
            .Where(articleBarcode => articleBarcode.BarCode == barcode)
            .Select(articleBarcode => articleBarcode.Art)
            .FirstOrDefaultAsync();

        article ??= await ctx.Articles
            .AsNoTracking()
            .FirstOrDefaultAsync(article => article.Nr == barcode);

        if (article is null)
            return null;

        var unitName = article.UnitId.HasValue
            ? await ctx.UnitsOfMeasure
                .AsNoTracking()
                .Where(unit => unit.Id == article.UnitId.Value)
                .Select(unit => unit.Name)
                .FirstOrDefaultAsync()
            : null;

        var image = await ctx.ArticlesImages
            .AsNoTracking()
            .Where(articleImage => articleImage.ArtId == article.Id)
            .Select(articleImage => articleImage.Image)
            .FirstOrDefaultAsync();

        return new ArticleItem
        {
            ArticleId = article.Id,
            Nr = article.Nr,
            Desc = article.Description,
            Barcode = barcode,
            Details = $"{article.Nr ?? ""} {article.Description ?? ""}".Trim(),
            UnitName = unitName,
            DocImage = image,
            DocBase64Image = image is null ? null : Convert.ToBase64String(image)
        };
    }

    public async Task<bool> SetArticleLocation(int userId, int articleId, SetArticleLocationRequest request)
    {
        try
        {
            await ctx.Procedures.SpXama_CreateCheckingStocksOrMovingArticlesAsync(
                articleId,
                request.OldShelfId,
                request.NewShelfId,
                request.OldQuantity,
                request.NewQuantity,
                userId);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<SaveArticleImageResponse> SaveArticleImage(
        int articleId,
        SaveArticleImageRequest request)
    {
        var article = await ctx.Articles
            .FirstOrDefaultAsync(article => article.Id == articleId);

        if (article is null)
        {
            return new SaveArticleImageResponse
            {
                ArticleId = -1
            };
        }

        var imageBytes = string.IsNullOrWhiteSpace(request.DocBase64Image)
            ? null
            : Convert.FromBase64String(request.DocBase64Image);

        if (imageBytes is null || imageBytes.Length == 0)
        {
            return new SaveArticleImageResponse
            {
                ArticleId = -1
            };
        }

        var articleImage = new ArticlesImages
        {
            Art = article,
            Image = imageBytes
        };

        await ctx.ArticlesImages.AddAsync(articleImage);
        await ctx.SaveChangesAsync();

        return new SaveArticleImageResponse
        {
            ArticleId = article.Id
        };
    }
}
