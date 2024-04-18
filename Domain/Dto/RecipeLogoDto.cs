namespace Domain.Dto
{
    public class RecipeLogoDto
    {
        public Guid? Id { get; set; }
        public byte[]? Logo { get; set; }
        public string? FileName { get; set; }
        public string? FileFormat { get; set; }
        public string? MimeType { get; set; }


    }
}
