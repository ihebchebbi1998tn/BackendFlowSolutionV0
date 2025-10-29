using MyApi.Modules.Articles.DTOs;

namespace MyApi.Modules.Articles.Services
{
    public interface IArticleService
    {
        Task<ArticleListDto> GetAllArticlesAsync(string? type, string? category, string? status, string? location, string? search, int page, int limit, string? sortBy, string? sortOrder);
        Task<ArticleDto?> GetArticleByIdAsync(string id);
        Task<ArticleDto> CreateArticleAsync(CreateArticleDto dto, string userId);
        Task<ArticleDto?> UpdateArticleAsync(string id, UpdateArticleDto dto, string userId);
        Task<bool> DeleteArticleAsync(string id);
        Task<InventoryTransactionDto> CreateTransactionAsync(CreateInventoryTransactionDto dto, string userId);
        Task<List<InventoryTransactionDto>> GetArticleTransactionsAsync(string articleId);
        Task<BatchOperationResultDto> BatchUpdateStockAsync(BatchUpdateStockDto dto, string userId);
        Task<List<ArticleCategoryDto>> GetAllCategoriesAsync();
        Task<ArticleCategoryDto> CreateCategoryAsync(CreateArticleCategoryDto dto);
        Task<List<LocationDto>> GetAllLocationsAsync();
        Task<LocationDto> CreateLocationAsync(CreateLocationDto dto);
    }
}
