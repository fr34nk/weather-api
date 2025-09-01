namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class IWeather
    {
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }

    public class WeatherDto
    {
        public int? Id { get; set; }
        public DateTime Time { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
