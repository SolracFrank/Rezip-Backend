using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class RecipeLogoConfiguration : IEntityTypeConfiguration<RecipeLogo>
    {
        public void Configure(EntityTypeBuilder<RecipeLogo> builder)
        {
            builder.HasKey(e => e.RecipeId).HasName("PRIMARY");

            builder.ToTable("recipe_logo");

            builder.Property(e => e.RecipeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("recipe_id");
            builder.Property(e => e.FileFormat)
                .HasMaxLength(50)
                .HasColumnName("file_format");
            builder.Property(e => e.Logo)
                .HasColumnType("mediumblob")
                .HasColumnName("logo");
            builder.Property(e => e.MimeType)
                .HasMaxLength(50)
                .HasColumnName("mime_type");

            builder.HasOne(d => d.Recipe).WithOne(p => p.RecipeLogo)
                .HasForeignKey<RecipeLogo>(d => d.RecipeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_logo_recipe");
        }
    }
}
