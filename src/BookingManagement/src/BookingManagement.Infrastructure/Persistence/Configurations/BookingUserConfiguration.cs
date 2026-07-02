using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.Persistence.Configurations;

public sealed class BookingUserConfiguration : IEntityTypeConfiguration<BookingUser>
{
    public void Configure(EntityTypeBuilder<BookingUser> builder)
    {
        builder.ToTable("BookingUsers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(100)
            .ValueGeneratedNever();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasData(
            new { Id = "user-001", DisplayName = "Laith Amro" },
            new { Id = "user-002", DisplayName = "Sara Ahmad" },
            new { Id = "user-003", DisplayName = "Omar Khaled" });
    }
}