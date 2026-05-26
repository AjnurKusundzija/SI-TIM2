namespace TelecomSupportSystem.BLL.DTOs.Faq
{
    public class UpdateFaqDto
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int SortOrder { get; set; }
    }
}
