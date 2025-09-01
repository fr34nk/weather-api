using System.Collections;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public enum DataFields
    {
        temperature_2m,
        relative_humidity_2m,
        wind_speed_10m
    }

    public class GetForecastDaysDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int PastDays { get; set; }
    }
}
