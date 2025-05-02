using Microsoft.EntityFrameworkCore;
using TechGear.ChatService.Data;
using TechGear.ChatService.DTOs;
using TechGear.ChatService.Interfaces;
using TechGear.ChatService.Models;

namespace TechGear.ChatService.Services
{
    public class MessageService(TechGearChatServiceContext context, IHttpClientFactory httpClientFactory) : IMessageService
    {
        private readonly TechGearChatServiceContext _context = context;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<IEnumerable<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(int senderId, int receiverId)
        {
            return await _context.Messages
                .CountAsync(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead);
        }

        public async Task<MessageDto> SendMessageAsync(MessageDto message)
        {
            var newMessage = new Message
            {
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                IsImage = message.IsImage,
                ImageUrl = message.ImageUrl,
                IsRead = false,
                SentAt = message.SentAt,
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return new MessageDto
            {
                Id = newMessage.Id,
                SenderId = newMessage.SenderId,
                ReceiverId = newMessage.ReceiverId,
                Content = newMessage.Content,
                IsImage = newMessage.IsImage,
                ImageUrl = newMessage.ImageUrl,
                IsRead = newMessage.IsRead,
                SentAt = newMessage.SentAt,
            };
        }

        public async Task MarkMessageAsReadAsync(int senderId, int receiverId)
        {
            var messages = await _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToListAsync();

            if (messages.Count == 0)
            {
                return;
            }

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatUserDto>?> GetChatUsersAsync()
        {
            var userId = 1;

            var userIds = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct() 
                .ToListAsync();

            var client = _httpClientFactory.CreateClient("ApiGatewayClient");
            var response = await client.PostAsJsonAsync("api/v1/users/user-names", userIds);

            if (!response.IsSuccessStatusCode) return null;

            var userNames = await response.Content.ReadFromJsonAsync<List<string>>();
            var chatUsers = new List<ChatUserDto>();

            var unreadCounts = await _context.Messages
                .Where(m => m.ReceiverId == userId && userIds.Contains(m.SenderId) && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var allMessages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId && userIds.Contains(m.ReceiverId)) ||
                    (m.ReceiverId == userId && userIds.Contains(m.SenderId)))
                .ToListAsync();

            var lastMessages = allMessages
                .GroupBy(m =>
                    m.SenderId < m.ReceiverId
                        ? new { User1 = m.SenderId, User2 = m.ReceiverId }
                        : new { User1 = m.ReceiverId, User2 = m.SenderId })
                .Select(g => g.OrderByDescending(m => m.SentAt).First())
                .ToDictionary(
                    m => m.SenderId == userId ? m.ReceiverId : m.SenderId,
                    m => m);

            for (int i = 0; i < userIds.Count; i++)
            {
                var uid = userIds[i];
                lastMessages.TryGetValue(uid, out var lastMessage);

                Console.WriteLine(lastMessage!.SenderId);

                chatUsers.Add(new ChatUserDto
                {
                    Id = uid,
                    UserName = userNames![i],
                    UnreadMessageCount = unreadCounts.GetValueOrDefault(uid, 0),
                    LastMessageSentAt = lastMessage?.SentAt ?? DateTime.MinValue,
                    LastMessagePreview = lastMessage?.Content,
                    IsImage = lastMessage?.IsImage ?? false,
                    SenderId = lastMessage!.SenderId
                });
            }

            return chatUsers;
        }
    }
}
