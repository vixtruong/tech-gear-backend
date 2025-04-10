
using Microsoft.EntityFrameworkCore;
using TechGear.AuthService.Data;
using TechGear.AuthService.Interfaces;

namespace TechGear.AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<TechGearAuthServiceContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddScoped<IAuthService, Services.AuthService>();

            builder.Services.AddHttpClient("UserServiceClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5006");
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
