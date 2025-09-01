using System.Text.Json.Serialization;

namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{

    public class HourlyForecastDto
    {
        public DateTime[] time { get; set; }
        public int[] temperature_2m { get; set; }
        public int[] relative_humidity_2m { get; set; }
        public int[] wind_speed_10m { get; set; }
    }

    public class HourlyForecastResponseTree
    {
        public double latitude { get; }
        public double longitude { get; }
        public double generation_ms { get; }
        public int utc_offset_seconds { get; }
        public string timezone { get; }
        public string timezone_abbreviation { get; }
        public double elevation { get; }

        public class Current
        {

            public string time { get;  }
            public string interval { get;  }
            public string temperature_2m { get;  }
            public string relative_humidity_2m { get;  }
            public string wind_speed_10m { get;  }
            public string wind_direction_10m { get;  }
        }


        public class CurrentUnits
        {
            public string time { get; }
            public int interval { get; }
            public double temperature_2m { get; }
            public int relative_humidity_2m { get; }
            public double wind_speed_10m { get; }
            public int wind_direction_10m { get; }
        }

        public CurrentUnits current_units { get; set; }

        public Current current { get; set; }

        public class Hourly
        {


            public string[] time { get; }
            public int[] temperature_2m { get; }
            public int[] relative_humidity_2m { get; }
            public int[] wind_speed_10m { get; }
        }

        public Hourly hourly { get; set; }
    }
}


