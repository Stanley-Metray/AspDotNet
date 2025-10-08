using System;
namespace AspDotNet.Models
{
    public class ChatMessageModel
    {
        public string Message { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}

