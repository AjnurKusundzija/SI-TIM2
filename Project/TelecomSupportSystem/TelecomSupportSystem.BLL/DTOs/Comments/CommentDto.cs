namespace TelecomSupportSystem.BLL.DTOs.Comments
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public bool IsSystemMessage { get; set; }
        public IEnumerable<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto> Attachments { get; set; } = new List<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto>();
    }
}