namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class PackageFeatureDto
    {
        public int FeatureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
