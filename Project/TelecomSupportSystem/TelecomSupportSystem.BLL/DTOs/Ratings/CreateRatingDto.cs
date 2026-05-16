using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Ratings
{
    public class CreateRatingDto
    {
        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }

        public string? RatingComment { get; set; }
    }
}
