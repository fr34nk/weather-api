using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace HappyCode.NetCoreBoilerplate.Core.Models
{
    [Table("weather", Schema = "forecast")]
    public class Weather
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int Id { get; set; }

        [Column("time_str", TypeName = "date")]
        public DateTime Time { get; set; }

        [Required]
        [Column("temperature", TypeName = "int(11)")]
        public int Temperature { get; set; }

        [Required]
        [Column("humidity", TypeName = "int(11)")]
        public int Humidity { get; set; }

        [Required]
        [Column("wind_speed", TypeName = "int(11)")]
        public int WindSpeed { get; set; }

    }
}
