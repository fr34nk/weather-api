using HappyCode.NetCoreBoilerplate.Core.Providers;
using HappyCode.NetCoreBoilerplate.Core.Repositories;
using HappyCode.NetCoreBoilerplate.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HappyCode.NetCoreBoilerplate.Core.Registrations
{
    public static class CoreRegistrations
    {
        public static IServiceCollection AddCoreComponents(this IServiceCollection services)
        {
            services.AddTransient<IGoogleCalendarRepository, GoogleCalendarRepository>();
            services.AddTransient<IWeatherRepository, WeatherRepository>();
            services.AddScoped<ICarService, CarService>();
            services.AddSingleton<VersionProvider>();

            return services;
        }
    }
}
