using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using HappyCode.NetCoreBoilerplate.Core;
using MySqlX.XDevAPI.Common;



namespace HappyCode.NetCoreBoilerplate.Core.Services
{
    public class ForecastRequest
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int PastDays { get; set; } = 0;

        public string[] Hourly { get; set; } = Array.Empty<string>();

        public string ToQueryString()
        {
            var HourlyParam = string.Join(",", Hourly);
            return $"?latitude={Latitude}&longitude={Longitude}&past_days={PastDays}&hourly={HourlyParam}&current=temperature_2m,relative_humidity_2m,rain,wind_speed_10m&forecast_days=3";
        }
    }

    public class HourlyUnits
    {
        public string Time { get; set; } = string.Empty;
        public string Temperature_2m { get; set; } = string.Empty;
        public string Relative_humidity_2m { get; set; } = string.Empty;
        public string Wind_speed_10m { get; set; } = string.Empty;
    }

    public class HourlyData
    {
        public List<string> Time { get; set; } = new();
        public List<double> Temperature_2m { get; set; } = new();
        public List<int> Relative_humidity_2m { get; set; } = new();
        public List<double> Wind_speed_10m { get; set; } = new();
    }

    public class ForecastResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double GenerationtimeMs { get; set; }
        public int UtcOffsetSeconds { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public string TimezoneAbbreviation { get; set; } = string.Empty;
        public double Elevation { get; set; }
        public HourlyUnits HourlyUnits { get; set; } = new();
        public HourlyData Hourly { get; set; } = new();
    }

    public interface IWeatherService
    {
        public Task<ForecastResponse?> GetForecastAsync(
            Double Latitude,
            Double Longitude,
            int PastDays,
            string[] Hourly
        );
    }

    public class WeatherService : IWeatherService
    {
        private readonly WeatherContext _dbContext;
        private readonly HttpClient _httpClient;

        private readonly IWeatherService _weatherService;

        public WeatherService(WeatherContext dbContext, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
        }

        public async Task EnsureDatabaseConnectionAsync()
        {
            if (!await this._dbContext.Database.CanConnectAsync())
            {
                throw new InvalidOperationException("❌ Could not connect to the database. Check your connection string and DB server.");
            }
        }

        public async Task<ForecastResponse?> GetForecastAsync(
            Double Latitude,
            Double Longitude,
            int PastDays,
            string[] Hourly
            )
        {
            var requestParams = new ForecastRequest
            {
                Latitude = Latitude,
                Longitude = Longitude,
                PastDays = PastDays,
                Hourly = Hourly
            };

            Console.Write("requestParmas; ");
            Console.Write(requestParams.PastDays);

            var url = "forecast" + requestParams.ToQueryString();
            var result = await _httpClient.GetFromJsonAsync<ForecastResponse>(url);

            Console.Write(result.ToString());

            return result;
        }
    }
}
