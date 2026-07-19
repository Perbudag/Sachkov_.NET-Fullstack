using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.Property(d => d.Id).HasColumnName("department_id");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGTH)
            .HasConversion(
                src => src.Value,
                dst => Name.Create(dst).Value);

        builder.Property(d => d.Slug)
            .HasColumnName ("slug")
            .IsRequired()
            .HasMaxLength(Slug.MAX_LENGTH)
            .HasConversion(
                src => src.Value,
                dst => Slug.Create(dst).Value);

        builder.Property(d => d.Path)
            .HasColumnName("path")
            .IsRequired()
            .HasConversion(
                src => src.ToString(),
                dst => Path.Create(dst).Value);

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(d => d.ParentId).HasColumnName("parent_id");
        builder.HasMany<Department>()
            .WithOne()
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany<DepartmentLocation>()
            .WithOne()
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<DepartmentPosition>()
            .WithOne()
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(d => d.Name).IsUnique();
    }
}
