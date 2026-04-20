using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class Izvjestaj
{
    public int IzvjestajId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public TipIzvjestaja TipIzvjestaja { get; set; }
    public DateTime PeriodOd { get; set; }
    public DateTime PeriodDo { get; set; }
    public DateTime DatumGenerisanja { get; set; }
}
