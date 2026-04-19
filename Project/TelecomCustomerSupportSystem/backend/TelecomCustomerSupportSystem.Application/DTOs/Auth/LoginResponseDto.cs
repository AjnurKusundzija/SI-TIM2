namespace TelecomCustomerSupportSystem.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime IstekTokena { get; set; }
    public string KorisnickoIme { get; set; } = string.Empty;
    public string Uloga { get; set; } = string.Empty;
}