using System;
using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Subscriptions
{
    public class AssignSubscriptionDto
    {
        [Required(ErrorMessage = "Paket je obavezan.")]
        public int CatalogPackageId { get; set; }

        [Required(ErrorMessage = "Datum početka je obavezan.")]
        public DateTime StartDate { get; set; }
    }
}
