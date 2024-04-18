namespace Infrastructure.Data;
using Domain.Entities;

using Microsoft.EntityFrameworkCore;


public class RecipesDbContext : DbContext
{
    public RecipesDbContext()
    {
    }

    public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeLogo> RecipeLogos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFavoritesRecipe> UserFavoritesRecipes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecipesDbContext).Assembly);
    }

}
