namespace Domain.Entities;

public partial class UserFavoritesRecipe
{
    public Guid Id { get; set; }

    public Guid? RecipeId { get; set; }

    public string SubId { get; set; } = null!;

    public virtual Recipe? Recipe { get; set; }

    public virtual User Sub { get; set; } = null!;
}
