namespace Domain.Entities;

public partial class Comment
{
    public Guid Id { get; set; }

    public string Comment1 { get; set; } = null!;

    public string? Owner { get; set; }

    public Guid? Response { get; set; }

    public Guid Recipe { get; set; }

    public virtual ICollection<Comment> InverseResponseNavigation { get; set; } = new List<Comment>();

    public virtual User? OwnerNavigation { get; set; }

    public virtual Recipe RecipeNavigation { get; set; } = null!;

    public virtual Comment? ResponseNavigation { get; set; }
}
