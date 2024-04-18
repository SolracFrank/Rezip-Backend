namespace Domain.Entities;

public partial class Recipe
{
    public Guid RecipeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Procedures { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual RecipeLogo? RecipeLogo { get; set; }

    public virtual ICollection<UserFavoritesRecipe> UserFavoritesRecipes { get; set; } = new List<UserFavoritesRecipe>();
}
