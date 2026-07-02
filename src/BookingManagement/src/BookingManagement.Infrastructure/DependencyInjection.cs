using BookingManagement.Application.Abstractions;
using BookingManagement.Infrastructure.Concurrency;
using BookingManagement.Infrastructure.Persistence;
using BookingManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BookingDb")
            ?? throw new InvalidOperationException("Connection string 'BookingDb' was not found.");

        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<BookingDbContext>());

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IBookingUserRepository, BookingUserRepository>();
        services.AddScoped<IResourceBookingLock, SqlServerResourceBookingLock>();

        return services;
    }
}