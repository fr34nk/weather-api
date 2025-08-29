namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class WeatherDetailsDto
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int Humidity  { get; set; }
        public int WindSpeed { get; set; }
        public int Temperature { get; set; }
    }
}
