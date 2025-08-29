using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Models;

namespace HappyCode.NetCoreBoilerplate.Core.Extensions
{
    internal static class WeatherExtensions
    {
        public static WeatherDto MapToDto(this Weather source)
        {
            return new WeatherDto
            {
                Id = source.Id,
                Humidity = source.Humidity,
                Time = source.Time,
                Temperature = source.Temperature,
                WindSpeed = source.WindSpeed,
            };
        }
    }
}
