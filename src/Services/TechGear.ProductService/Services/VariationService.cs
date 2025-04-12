using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace TechGear.ProductService.Services
{
    public class VariationService(TechGearProductServiceContext context) : IVariationService
    {
        private readonly TechGearProductServiceContext _context = context;

        public async Task<IEnumerable<Variation>> GetAllVariationsAsync()
        {
            return await _context.Variations.ToListAsync();
        }

        public async Task<Variation?> GetVariationByIdAsync(int variationId)
        {
            return await _context.Variations.FirstOrDefaultAsync(v => v.Id == variationId);
        }

        public async Task<Variation?> GetVariationByNameAsync(string variationName)
        {
            return await _context.Variations.FirstOrDefaultAsync(v => v.Name == variationName);
        }

        public async Task<IEnumerable<Variation>> GetVariationsByCateIdAsync(int cateId)
        {
            return await _context.Variations.Where(v => v.CategoryId == cateId).ToListAsync();
        }

        public async Task<Variation?> AddVariationAsync(Variation variation)
        {
            var existVariation = await _context.Variations.FirstOrDefaultAsync(v => v.Name == variation.Name);
            if (existVariation != null) return null;

            _context.Variations.Add(variation);
            await _context.SaveChangesAsync();

            return variation;
        }

        public async Task<bool> UpdateVariationAsync(Variation variation)
        {
            var existVariation = await _context.Variations.FindAsync(variation.Id);

            if (existVariation == null) return false;

            existVariation.Name = variation.Name;
            existVariation.CategoryId = variation.CategoryId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVariationByIdAsync(int variationId)
        {
            var existVariation = await _context.Variations.FindAsync(variationId);

            if (existVariation == null) return false;

            _context.Variations.Remove(existVariation);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
