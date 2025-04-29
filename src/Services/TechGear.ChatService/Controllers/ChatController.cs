using Microsoft.AspNetCore.Mvc;
using TechGear.ChatService.DTOs;
using TechGear.ChatService.Interfaces;
using TechGear.ChatService.WebSockets;

namespace TechGear.ChatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly WebSocketConnectionManager _webSocketManager;

        public ChatController(IMessageService messageService, WebSocketConnectionManager webSocketManager)
        {
            _messageService = messageService;
            _webSocketManager = webSocketManager;
        }

        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            if (senderId <= 0 || receiverId <= 0)
            {
                return BadRequest("Invalid sender or receiver ID.");
            }

            var messages = await _messageService.GetMessagesAsync(senderId, receiverId);
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto? message)
        {
            if (message == null || message.SenderId <= 0 || message.ReceiverId <= 0)
            {
                return BadRequest("Invalid message data.");
            }

            var newMessage = await _messageService.SendMessageAsync(message);

            var messageJson = System.Text.Json.JsonSerializer.Serialize(newMessage);
            await _webSocketManager.SendMessageToUser(newMessage.ReceiverId.ToString(), messageJson);
            await _webSocketManager.SendMessageToUser(newMessage.SenderId.ToString(), messageJson);

            return Ok("Message sent successfully.");
        }

        [HttpGet("unread-count/{senderId}/{receiverId}")]
        public async Task<IActionResult> GetUnreadMessageCount(int senderId, int receiverId)
        {
            if (senderId <= 0 || receiverId <= 0)
            {
                return BadRequest("Invalid sender or receiver ID.");
            }

            var count = await _messageService.GetUnreadMessageCountAsync(senderId, receiverId);
            return Ok(count);
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessageAsRead([FromBody] MarkAsReadDto dto)
        {
            if (dto.SenderId <= 0 || dto.ReceiverId <= 0)
            {
                return BadRequest("Invalid sender or receiver ID.");
            }

            await _messageService.MarkMessageAsReadAsync(dto.SenderId, dto.ReceiverId);
            return Ok("Messages marked as read successfully.");
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUserInfos()
        {
            var users = await _messageService.GetChatUsersAsync();

            return Ok(users);
        }
    }
}