using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class PositionConfigurations : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.Property(l => l.Id).HasColumnName("position_id");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGTH)
            .HasConversion(
                src => src.Value,
                dst => Name.Create(dst));

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasMany(l => l.Departments)
            .WithOne()
            .HasForeignKey(dl => dl.PositionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
