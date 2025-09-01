using System.ComponentModel.DataAnnotations;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class WeatherPostDto
    {
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public double Humidity { get; set; }
        [Required]
        public double WindSpeed { get; set; }

        [Required]
        public double Temperature { get; set; }
    }
}
