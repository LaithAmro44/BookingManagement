using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.Persistence.Configurations;

public sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(100)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasData(
            new { Id = "meeting-room-a", Name = "Meeting Room A" },
            new { Id = "meeting-room-b", Name = "Meeting Room B" },
            new { Id = "projector-1", Name = "Projector 1" });
    }
}