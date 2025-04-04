using ClickCounter.Application.Services.Counter;
using Microsoft.Extensions.DependencyInjection;

namespace ClickCounter.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplication(this IServiceCollection services) {
        services.AddScoped<ICounterService, CounterService>();

        return services;
    }
}