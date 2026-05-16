namespace TelecomSupportSystem.BLL.DTOs.Ratings
{
    public class RatingDto
    {
        public int RatingValue { get; set; }
        public string RatingComment { get; set; } = string.Empty;
        public DateTime RatingDate { get; set; }
        public int UserId { get; set; }
    }
}
