using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace HappyCode.NetCoreBoilerplate.Core.Models
{
    [Table("calendar_event", Schema = "forecast")]
    public class GoogleCalendarModel
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int id { get; set; }

        [Column("event_id", TypeName = "string")]
        public string eventId { get; set; }

        [Required]
        [Column("data", TypeName = "data")]
        public string data { get; set; }
    }
}
