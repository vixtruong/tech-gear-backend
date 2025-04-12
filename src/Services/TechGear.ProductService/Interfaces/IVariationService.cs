using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IVariationService
    {
        Task<IEnumerable<Variation>> GetAllVariationsAsync();
        Task<Variation?> GetVariationByIdAsync(int variationId);
        Task<Variation?> GetVariationByNameAsync(string variationName);
        Task<IEnumerable<Variation>> GetVariationsByCateIdAsync(int cateId);
        Task<Variation?> AddVariationAsync(Variation variation);
        Task<bool> UpdateVariationAsync(Variation variation);
        Task<bool> DeleteVariationByIdAsync(int variationId);
    }
}