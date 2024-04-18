namespace Domain.Entities;

public partial class RecipeLogo
{
    public Guid RecipeId { get; set; }

    public byte[]? Logo { get; set; }

    public string MimeType { get; set; } = null!;

    public string FileFormat { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}
