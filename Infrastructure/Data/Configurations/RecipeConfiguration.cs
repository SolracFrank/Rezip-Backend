namespace Infrastructure.Data.Configurations;
using Domain.Entities;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


internal class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(e => e.RecipeId).HasName("PRIMARY");
        builder.ToTable("recipe");
        builder.HasIndex(e => e.CreatedBy, "fk_recipes_user_idx");

        builder.Property(e => e.RecipeId).HasColumnName("recipe_id");
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(250)
            .HasColumnName("created_by");
        builder.Property(e => e.Description)
            .HasMaxLength(255)
            .HasColumnName("description");
        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");
        builder.Property(e => e.Procedures)
            .HasColumnType("text")
            .HasColumnName("procedures");
        builder.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Recipes)
            .HasForeignKey(d => d.CreatedBy)
            .HasConstraintName("fk_recipes_user");
    }
}
