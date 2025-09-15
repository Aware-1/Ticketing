using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "محتوای پیام الزامی است")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsFromSupport { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserDisplayName { get; set; } = string.Empty;

        public int TicketId { get; set; }

        // Additional properties for UI
        public string FormattedCreatedAt { get; set; } = string.Empty;
        public string RelativeTime { get; set; } = string.Empty;
        public bool IsCurrentUser { get; set; }
        public string CssClass { get; set; } = string.Empty;
    }

    public class CreateMessageDTO
    {
        [Required(ErrorMessage = "محتوای پیام الزامی است")]
        [StringLength(5000, ErrorMessage = "پیام نباید بیش از 5000 کاراکتر باشد")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "شناسه تیکت الزامی است")]
        public int TicketId { get; set; }

        public List<int>? AttachmentIds { get; set; }
    }

    public class UpdateMessageDTO
    {
        [Required(ErrorMessage = "محتوای پیام الزامی است")]
        [StringLength(5000, ErrorMessage = "پیام نباید بیش از 5000 کاراکتر باشد")]
        public string Content { get; set; } = string.Empty;
    }

    public class MessageListDTO
    {
        public List<MessageDTO> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class MessageNotificationDTO
    {
        public int MessageId { get; set; }
        public int TicketId { get; set; }
        public string TicketSubject { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsFromSupport { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class ChatMessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedAtFormatted { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public bool IsFromSupport { get; set; }
        public int UserId { get; set; }
        public bool IsCurrentUser { get; set; }
        public string AvatarClass { get; set; } = string.Empty;
        public string MessageClass { get; set; } = string.Empty;
        public List<AttachmentDTO>? Attachments { get; set; }
    }

    public class AttachmentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FormattedFileSize { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
        public bool IsImage { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
    }
}
