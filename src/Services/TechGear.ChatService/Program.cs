using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TechGear.ChatService.Data;
using TechGear.ChatService.DTOs;
using TechGear.ChatService.Interfaces;
using TechGear.ChatService.Services;
using TechGear.ChatService.WebSockets;

namespace TechGear.ChatService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<TechGearChatServiceContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Query["token"];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
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
            builder.Services.AddSingleton<WebSocketConnectionManager>();
            builder.Services.AddScoped<IMessageService, MessageService>();

            builder.Services.AddHttpClient("ApiGatewayClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseWebSockets();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Map("/wss", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (!authResult.Succeeded)
                    {
                        Console.WriteLine($"Authentication failed: {authResult.Failure?.Message}");
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid or missing token");
                        return;
                    }

                    var userId = context.Request.Query["userId"];
                    if (string.IsNullOrEmpty(userId))
                    {
                        Console.WriteLine("Missing userId in WebSocket request");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Missing userId");
                        return;
                    }

                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var socketManager = context.RequestServices.GetRequiredService<WebSocketConnectionManager>();

                    socketManager.AddSocket(webSocket, userId!);

                    socketManager.OnReceiveText += async (uid, message) =>
                    {
                        Console.WriteLine($"Received text from {uid}: {message}");
                        var messageService = context.RequestServices.GetRequiredService<IMessageService>();
                        // Giả sử message là JSON, cần parse để lưu vào DB
                        try
                        {
                            var messageDto = System.Text.Json.JsonSerializer.Deserialize<MessageDto>(message);
                            await messageService.SendMessageAsync(messageDto!);
                            await socketManager.SendMessageToUser(messageDto!.ReceiverId.ToString(), message);
                            await socketManager.SendMessageToUser(messageDto.SenderId.ToString(), message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing message from {uid}: {ex.Message}");
                        }
                    };

                    socketManager.OnReceiveBinary += (uid, payload) =>
                    {
                        Console.WriteLine($"Received binary from {uid}: {payload.Length} bytes");
                    };

                    await socketManager.ReceiveMessagesUntilCloseAsync(webSocket, userId!);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("WebSocket request expected");
                }
            });

            app.Run();
        }
    }
}