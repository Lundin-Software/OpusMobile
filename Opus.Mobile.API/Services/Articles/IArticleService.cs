using Opus.Mobile.Shared.Articles;

namespace Opus.Mobile.API.Services.Articles;

public interface IArticleService
{
    Task<ArticleItem?> GetArticleByBarcode(string barcode);

    Task<bool> SetArticleLocation(int userId, int articleId, SetArticleLocationRequest request);

    Task<SaveArticleImageResponse> SaveArticleImage(int articleId, SaveArticleImageRequest request);
}
