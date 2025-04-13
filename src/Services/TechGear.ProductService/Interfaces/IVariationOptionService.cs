using TechGear.ProductService.DTOs;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Interfaces
{
    public interface IVariationOptionService
    {
        Task<IEnumerable<VariationOption>> GetAllVariationOptionsAsync();
        Task<IEnumerable<VariationOption>> GetAllVariationOptionsByVariationIdAsync(int variationId);
        Task<VariationOption?> GetVariationOptionByIdAsync(int optionId);
        Task<VariationOption?> GetVariationOptionByValueAsync(string value);
        Task<VariationOption?> AddVariationOptionAsync(VariationOptionDto variationOption);
        Task<bool> UpdateVariationOptionAsync(VariationOptionDto variationOption);
        Task<bool> RemoveVariationOptionAsync(int optionId);
    }
}
