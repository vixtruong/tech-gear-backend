using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;

namespace TechGear.ProductService.Services
{
    public class VariationOptionService(TechGearProductServiceContext context) : IVariationOptionService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<VariationOption>> GetAllVariationOptionsAsync()
        {
            return await _context.VariationOptions.ToListAsync();
        }

        public async Task<IEnumerable<VariationOption>> GetAllVariationOptionsByVariationIdAsync(int variationId)
        {
            return await _context.VariationOptions.Where(vo => vo.VariationId == variationId).ToListAsync();
        }

        public async Task<VariationOption?> GetVariationOptionByIdAsync(int optionId)
        {
            return await _context.VariationOptions.FirstOrDefaultAsync(vo => vo.Id == optionId);
        }

        public async Task<VariationOption?> GetVariationOptionByValueAsync(string value)
        {
            return await _context.VariationOptions.FirstOrDefaultAsync(vo => vo.Value == value);
        }

        public async Task<VariationOption?> AddVariationOptionAsync(VariationOption variationOption)
        {
            var existOption =
                await _context.VariationOptions.FirstOrDefaultAsync(vo => vo.Value == variationOption.Value);

            if (existOption != null) return null;

            _context.VariationOptions.Add(variationOption);
            await _context.SaveChangesAsync();

            return variationOption;
        }

        public async Task<bool> UpdateVariationOptionAsync(VariationOption variationOption)
        {
            var existOption = await _context.VariationOptions.FindAsync(variationOption.Id);

            if (existOption == null) return false;

            existOption.Value = variationOption.Value;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveVariationOptionAsync(int optionId)
        {
            var existOption = await _context.VariationOptions.FindAsync(optionId);

            if (existOption == null) return false;

            _context.VariationOptions.Remove(existOption);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
