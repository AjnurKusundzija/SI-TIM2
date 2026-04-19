using TelecomCustomerSupportSystem.Domain.Enums;

namespace TelecomCustomerSupportSystem.Application.DTOs.Users;

public class UserDto
{
    public int KorisnikId { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public Uloga Uloga { get; set; }
    public StatusNaloga StatusNaloga { get; set; }
}