using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserFavoritesRecipeExtension : IEntityTypeConfiguration<UserFavoritesRecipe>
    {
        public void Configure(EntityTypeBuilder<UserFavoritesRecipe> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("user_favorites_recipes");

            builder.HasIndex(e => e.RecipeId, "fk_recipe_favorite_idx");

            builder.HasIndex(e => e.SubId, "fk_user_favorite_idx");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.RecipeId).HasColumnName("recipe_id");
            builder.Property(e => e.SubId)
                .HasMaxLength(250)
                .HasColumnName("sub_id");

            builder.HasOne(d => d.Recipe).WithMany(p => p.UserFavoritesRecipes)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("fk_recipe_favorite");

            builder.HasOne(d => d.Sub).WithMany(p => p.UserFavoritesRecipes)
                .HasForeignKey(d => d.SubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_favorite");
        }
    }
}
