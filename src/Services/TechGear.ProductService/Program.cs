
using Microsoft.EntityFrameworkCore;
using TechGear.ProductService.Data;
using TechGear.ProductService.Interfaces;
using TechGear.ProductService.Services;

namespace TechGear.ProductService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<TechGearProductServiceContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddControllers();

            // Register services
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, Services.ProductService>();
            builder.Services.AddScoped<IProductItemService, ProductItemService>();
            builder.Services.AddScoped<IProductConfigService, ProductConfigService>();
            builder.Services.AddScoped<IVariationService, VariationService>();
            builder.Services.AddScoped<IVariationOptionService, VariationOptionService>();
            builder.Services.AddScoped<IProductConfigService, ProductConfigService>();
            builder.Services.AddScoped<IRatingService, RatingService>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
