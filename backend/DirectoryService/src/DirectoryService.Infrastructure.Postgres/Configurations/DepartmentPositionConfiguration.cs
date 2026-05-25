using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("departments_positions");

        builder.Property(dp => dp.Id).HasColumnName("department_position_id");
        builder.HasKey(dp => dp.Id);

        builder.Property(dp => dp.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(dp => dp.PositionId)
            .HasColumnName("position_id")
            .IsRequired();

        builder.Property(dp => dp.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");
    }
}
