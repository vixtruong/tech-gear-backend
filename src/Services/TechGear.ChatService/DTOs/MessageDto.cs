namespace TechGear.ChatService.DTOs
{
    public class MessageDto
    {
        public int? Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string? Content { get; set; }

        public bool IsImage { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; }
    }
}
