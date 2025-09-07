using System.Collections;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public enum DataFields
    {
        temperature_2m,
        relative_humidity_2m,
        wind_speed_10m
    }

    public class GetWeatherReqDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int PastDays { get; set; }
        public int ForecastDays { get; set; }


        // public DateTime date { get; set; }

    }

    public class GetForecastDaysDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

    }

    public class GetRainyEventsRequestDto
        : GetForecastDaysDto
    {
        public string startDate { get; set; }

        // "2025-09-06T13:00"
        public string endDate { get; set;  }
    }

}
