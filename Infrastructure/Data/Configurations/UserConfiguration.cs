namespace Infrastructure.Data.Configurations;
using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {

        builder.HasKey(e => e.SubId).HasName("PRIMARY");

        builder.ToTable("user");

        builder.HasIndex(e => e.CreatedBy, "fk_user_creator_idx");

        builder.HasIndex(e => e.SubId, "sub_id_UNIQUE").IsUnique();

        builder.Property(e => e.SubId)
            .HasMaxLength(250)
            .HasColumnName("sub_id");
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(250)
            .HasColumnName("created_by");
        builder.Property(e => e.CreationDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("creation_date");
        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .HasColumnName("email");
        builder.Property(e => e.Lastname)
            .HasMaxLength(100)
            .HasColumnName("lastname");
        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .HasColumnName("name");
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
            .HasConstraintName("fk_user_creator");
    }
}
