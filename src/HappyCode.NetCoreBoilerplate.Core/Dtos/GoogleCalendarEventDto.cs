using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;


namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class GoogleCalendarEventDto
    {
        public int id { get; set; }
        public string eventId { get; set; }
        public string data { get; set; }
    }
}
