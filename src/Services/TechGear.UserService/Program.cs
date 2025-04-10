
using Microsoft.EntityFrameworkCore;
using TechGear.UserService.Data;
using TechGear.UserService.Interfaces;

namespace TechGear.UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<TechGearUserServiceContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers();

            builder.Services.AddScoped<IUserService, Services.UserService>();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
