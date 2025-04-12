
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            // Config Authentication with JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("Authorization")
                            .SetIsOriginAllowed(_ => true);
                    });
            });

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

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
