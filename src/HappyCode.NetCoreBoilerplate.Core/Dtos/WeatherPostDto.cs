using System.ComponentModel.DataAnnotations;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class WeatherPostDto
    {
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int Humidity { get; set; }
        [Required]
        public int WindSpeed { get; set; }

        [Required]
        public int Temperature { get; set; }
    }
}
