namespace TelecomSupportSystem.BLL.DTOs.Auth
{
    public class RefreshResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
