using TechGear.ChatService.DTOs;
using TechGear.ChatService.Models;

namespace TechGear.ChatService.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessagesAsync(int senderId, int receiverId);
        Task<int> GetUnreadMessageCountAsync(int senderId, int receiverId);
        Task<MessageDto> SendMessageAsync(MessageDto message);
        Task MarkMessageAsReadAsync(int senderId, int receiverId);
        Task<IEnumerable<ChatUserDto>?> GetChatUsersAsync();
    }
}
