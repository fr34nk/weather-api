using System.ComponentModel.DataAnnotations;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class WeatherPutDto
    {
        [Required]
        public int Humidity { get; set; }
    }
}
