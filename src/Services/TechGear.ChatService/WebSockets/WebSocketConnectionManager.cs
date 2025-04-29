using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TechGear.ChatService.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _userSockets = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(WebSocket socket, string userId)
        {
            // Đóng socket cũ nếu tồn tại
            if (_userSockets.TryGetValue(userId, out var oldSocket) && oldSocket.State == WebSocketState.Open)
            {
                oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Replaced by new connection", CancellationToken.None).GetAwaiter().GetResult();
            }
            _userSockets.TryAdd(userId, socket);
        }

        public void RemoveSocket(string userId)
        {
            _userSockets.TryRemove(userId, out _);
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (_userSockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task SendMessageToAllClients(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            foreach (var socket in _userSockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}