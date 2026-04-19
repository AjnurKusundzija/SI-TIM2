using TelecomCustomerSupportSystem.Domain.Enums;

namespace TelecomCustomerSupportSystem.Application.DTOs.Tickets;

public class CreateTicketDto
{
    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public Prioritet Prioritet { get; set; }
    public KategorijaProblema KategorijaProblema { get; set; }
    public int KreatorId { get; set; }
}