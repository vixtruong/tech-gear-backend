namespace TechGear.ChatService.DTOs
{
    public class ChatUserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? LastMessagePreview { get; set; }
        public bool IsImage { get; set; }
        public int SenderId { get; set; }
        public int UnreadMessageCount { get; set; }
        public DateTime LastMessageSentAt { get; set; }
    }
}
