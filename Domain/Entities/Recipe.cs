namespace Domain.Entities;
using System;


public partial class Recipe
{
    public Guid RecipeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Procedures { get; set; } = null!;

    public Guid CreatedBy { get; set; }
    public virtual User CreatedByNavigation { get; set; } = null!;
    public virtual RecipeLogo? RecipeLogo { get; set; }

}
