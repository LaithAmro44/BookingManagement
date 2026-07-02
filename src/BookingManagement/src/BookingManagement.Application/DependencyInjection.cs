using BookingManagement.Application.Bookings;
using BookingManagement.Application.ReferenceData;
using Microsoft.Extensions.DependencyInjection;

namespace BookingManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<ReferenceDataService>();

        return services;
    }
}