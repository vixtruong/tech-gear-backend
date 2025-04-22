namespace TechGear.UserService.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> AddToFavoritesAsync(int userId, int productId);
        Task<bool> RemoveFromFavoritesAsync(int userId, int productId);
        Task<List<int>> GetFavoriteProductsAsync(int userId);
        Task<bool> IsProductFavoriteAsync(int userId, int productId);
    }
}
