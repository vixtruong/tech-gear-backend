using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TechGear.ChatService.Data;
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
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid or missing token");
                        return;
                    }

                    var userId = context.Request.Query["userId"];
                    if (string.IsNullOrEmpty(userId))
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Missing userId");
                        return;
                    }

                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var socketManager = context.RequestServices.GetRequiredService<WebSocketConnectionManager>();

                    socketManager.AddSocket(webSocket, userId!);

                    await HandleWebSocketAsync(webSocket, socketManager, userId!);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });

            async Task HandleWebSocketAsync(WebSocket webSocket, WebSocketConnectionManager socketManager, string userId)
            {
                var buffer = new byte[1024 * 4];
                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            socketManager.RemoveSocket(userId);
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                            break;
                        }
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocket error for user {userId}: {ex.Message}");
                    socketManager.RemoveSocket(userId);
                    if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed due to error", CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error for user {userId}: {ex.Message}");
                    socketManager.RemoveSocket(userId);
                }
            }

            app.Run();
        }
    }
}