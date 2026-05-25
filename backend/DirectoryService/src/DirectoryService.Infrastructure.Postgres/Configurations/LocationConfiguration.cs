using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.Property(l => l.Id).HasColumnName("location_id");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(Name.MAX_LENGTH)
            .HasConversion(
                src => src.Value,
                dst => Name.Create(dst));

        builder.Property(l => l.Address)
            .HasColumnName("address")
            .IsRequired()
            .HasConversion(
                src => src.ToString(),
                dst => Address.Create(dst));

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasMany(l => l.Departments)
            .WithOne()
            .HasForeignKey(dl => dl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
