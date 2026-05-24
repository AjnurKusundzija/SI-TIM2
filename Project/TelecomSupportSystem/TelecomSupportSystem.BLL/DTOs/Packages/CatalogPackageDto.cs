namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class CatalogPackageDto
    {
        public int CatalogPackageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ActiveSubscriptionCount { get; set; }
    }
}
