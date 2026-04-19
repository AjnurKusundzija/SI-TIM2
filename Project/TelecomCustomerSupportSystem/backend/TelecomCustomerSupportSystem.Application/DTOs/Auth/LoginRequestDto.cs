namespace TelecomCustomerSupportSystem.Application.DTOs.Auth;

public class LoginRequestDto
{
    public string KorisnickoImeIliEmail { get; set; } = string.Empty;
    public string Lozinka { get; set; } = string.Empty;
}