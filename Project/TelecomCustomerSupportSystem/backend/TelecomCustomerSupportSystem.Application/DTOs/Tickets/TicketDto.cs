using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.DTOs.Tickets;

public class TicketDto
{
    public int TiketId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public StatusTiketa Status { get; set; }
    public Prioritet Prioritet { get; set; }
    public KategorijaProblema KategorijaProblema { get; set; }
    public DateTime DatumKreiranja { get; set; }
}