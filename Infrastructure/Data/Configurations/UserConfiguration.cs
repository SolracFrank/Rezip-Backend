namespace Infrastructure.Data.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.UserId).HasName("PRIMARY");

        builder.ToTable("user");

        builder.HasIndex(e => e.CreatedBy, "fk_user_creator_idx");

        builder.Property(e => e.UserId)
            .HasDefaultValueSql("UUID()")
            .HasColumnName("user_id");
        builder.Property(e => e.CreatedBy).HasColumnName("created_By");
        builder.Property(e => e.CreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("creation_date");
        builder.Property(e => e.Lastname)
            .HasMaxLength(100)
            .HasColumnName("lastname");
        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");
        builder.Property(e => e.Password)
            .HasMaxLength(25)
            .HasColumnName("password");
        builder.Property(e => e.UpdateDate)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("update_date");
        builder.Property(e => e.Username)
            .HasMaxLength(100)
            .HasColumnName("username");

        builder.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_user_creator");
    }
}
