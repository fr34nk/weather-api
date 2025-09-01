namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class WeatherDetailsDto
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public double Humidity  { get; set; }
        public double WindSpeed { get; set; }
        public double Temperature { get; set; }
    }
}
