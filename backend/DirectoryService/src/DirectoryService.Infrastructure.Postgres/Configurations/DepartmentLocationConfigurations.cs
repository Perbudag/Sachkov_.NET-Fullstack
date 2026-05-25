using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentLocationConfigurations : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("departments_locations");

        builder.Property(dl => dl.Id).HasColumnName("department_location_id");
        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dl => dl.LocationId)
            .HasColumnName("location_id")
            .IsRequired();

        builder.Property(dl => dl.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");
    }
}
