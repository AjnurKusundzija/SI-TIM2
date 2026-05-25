namespace TelecomSupportSystem.BLL.DTOs.Attachments
{
    public class FileUploadDto
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = System.Array.Empty<byte>();
        public long Size { get; set; } // Dodano polje koje servisi traže
    }
}