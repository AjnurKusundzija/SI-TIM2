namespace TelecomSupportSystem.BLL.DTOs.AI
{
    public class AgentSuggestionRequestDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<CommentSnippetDto> RecentComments { get; set; } = [];
    }

    public class CommentSnippetDto
    {
        public string AuthorRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
