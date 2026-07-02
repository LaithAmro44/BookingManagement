using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ResourceId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.StartDateTimeUtc)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(x => x.EndDateTimeUtc)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => new { x.ResourceId, x.Status, x.StartDateTimeUtc })
            .HasDatabaseName("IX_Bookings_Resource_Status_Start");

        builder.HasOne(x => x.Resource)
            .WithMany()
            .HasForeignKey(x => x.ResourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}