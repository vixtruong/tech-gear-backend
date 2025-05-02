using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TechGear.ChatService.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<WebSocket>> _userSockets = new ConcurrentDictionary<string, ConcurrentBag<WebSocket>>();
        private readonly int _receivePayloadBufferSize = 1024 * 4;

        public event Action<string, string>? OnReceiveText; // userId, message
        public event Action<string, byte[]>? OnReceiveBinary; // userId, payload

        public void AddSocket(WebSocket socket, string userId)
        {
            var sockets = _userSockets.GetOrAdd(userId, _ => new ConcurrentBag<WebSocket>());
            sockets.Add(socket);
            Console.WriteLine($"Added socket for user {userId}. Total sockets: {sockets.Count}");
        }

        public void RemoveSocket(string userId, WebSocket socket)
        {
            if (_userSockets.TryGetValue(userId, out var sockets))
            {
                var updatedSockets = new ConcurrentBag<WebSocket>(sockets.Where(s => s != socket && s.State != WebSocketState.Closed && s.State != WebSocketState.Aborted));
                if (updatedSockets.IsEmpty)
                {
                    _userSockets.TryRemove(userId, out _);
                    Console.WriteLine($"Removed all sockets for user {userId}.");
                }
                else
                {
                    _userSockets.TryUpdate(userId, updatedSockets, sockets);
                    Console.WriteLine($"Removed socket for user {userId}. Remaining sockets: {updatedSockets.Count}");
                }
            }
        }

        public async Task SendMessageToUser(string userId, string message)
        {
            if (_userSockets.TryGetValue(userId, out var sockets))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var tasks = new List<Task>();
                foreach (var socket in sockets.ToArray()) // ToArray để tránh lỗi sửa đổi collection
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
                    }
                    else
                    {
                        RemoveSocket(userId, socket); // Xóa socket đã đóng
                    }
                }
                await Task.WhenAll(tasks);
            }
        }

        public async Task SendMessageToAllClients(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var tasks = new List<Task>();
            foreach (var userSockets in _userSockets.ToArray()) // ToArray để tránh lỗi sửa đổi collection
            {
                foreach (var socket in userSockets.Value.ToArray())
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        tasks.Add(socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));
                    }
                    else
                    {
                        RemoveSocket(userSockets.Key, socket);
                    }
                }
            }
            await Task.WhenAll(tasks);
        }

        public async Task ReceiveMessagesUntilCloseAsync(WebSocket webSocket, string userId)
        {
            try
            {
                byte[] receivePayloadBuffer = new byte[_receivePayloadBufferSize];
                WebSocketReceiveResult webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);
                while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                {
                    byte[] webSocketMessagePayload = await ReceiveMessagePayloadAsync(webSocketReceiveResult, receivePayloadBuffer, webSocket);

                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Binary)
                    {
                        OnReceiveBinary?.Invoke(userId, webSocketMessagePayload);
                    }
                    else if (webSocketReceiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        OnReceiveText?.Invoke(userId, Encoding.UTF8.GetString(webSocketMessagePayload));
                    }

                    webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);
                }

                RemoveSocket(userId, webSocket);
                await webSocket.CloseAsync(webSocketReceiveResult.CloseStatus!.Value, webSocketReceiveResult.CloseStatusDescription, CancellationToken.None);
            }
            catch (WebSocketException wsex) when (wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                RemoveSocket(userId, webSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error for user {userId}: {ex.Message}");
                RemoveSocket(userId, webSocket);
                if (webSocket.State != WebSocketState.Closed && webSocket.State != WebSocketState.Aborted)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed due to error", CancellationToken.None);
                }
            }
        }

        private async Task<byte[]> ReceiveMessagePayloadAsync(WebSocketReceiveResult webSocketReceiveResult, byte[] receivePayloadBuffer, WebSocket webSocket)
        {
            byte[] messagePayload;

            if (webSocketReceiveResult.EndOfMessage)
            {
                messagePayload = new byte[webSocketReceiveResult.Count];
                Array.Copy(receivePayloadBuffer, messagePayload, webSocketReceiveResult.Count);
            }
            else
            {
                using (MemoryStream messagePayloadStream = new MemoryStream())
                {
                    messagePayloadStream.Write(receivePayloadBuffer, 0, webSocketReceiveResult.Count);
                    while (!webSocketReceiveResult.EndOfMessage)
                    {
                        webSocketReceiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);
                        messagePayloadStream.Write(receivePayloadBuffer, 0, webSocketReceiveResult.Count);
                    }

                    messagePayload = messagePayloadStream.ToArray();
                }
            }

            return messagePayload;
        }
    }
}