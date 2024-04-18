using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(e => e.Id).HasName("PRIMARY");

            builder.ToTable("comment");

            builder.HasIndex(e => e.Recipe, "fk_comment_recipe_idx");

            builder.HasIndex(e => e.Response, "fk_comment_response_idx");

            builder.HasIndex(e => e.Owner, "fk_comment_user_idx");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Comment1)
                .HasMaxLength(300)
                .HasColumnName("comment");
            builder.Property(e => e.Owner)
                .HasMaxLength(250)
                .HasColumnName("owner");
            builder.Property(e => e.Recipe).HasColumnName("recipe");
            builder.Property(e => e.Response).HasColumnName("response");

            builder.HasOne(d => d.OwnerNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Owner)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_comment_user");

            builder.HasOne(d => d.RecipeNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.Recipe)
                .HasConstraintName("fk_comment_recipe");

            builder.HasOne(d => d.ResponseNavigation).WithMany(p => p.InverseResponseNavigation)
                .HasForeignKey(d => d.Response)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_comment_response");
        }
    }
}
