﻿
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace TechGear.APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddAuthorization();
            builder.Services.AddOcelot();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}
