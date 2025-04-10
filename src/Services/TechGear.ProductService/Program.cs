
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

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
