using BookingManagement.Application.Abstractions;
using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.Persistence;

public sealed class BookingDbContext : DbContext, IUnitOfWork
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<BookingUser> BookingUsers => Set<BookingUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}