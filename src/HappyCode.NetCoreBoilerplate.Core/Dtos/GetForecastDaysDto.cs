using System.Collections;


namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{


    public class GetForecastDaysDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int PastDays { get; set; }

        public string[] DataFieldTypes = [ "temperature_2m", "relative_humidity_2m","wind_speed_10m" ];
    }
}
