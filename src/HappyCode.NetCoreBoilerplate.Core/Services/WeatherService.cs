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
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using HappyCode.NetCoreBoilerplate.Core.Models;
using Org.BouncyCastle.Asn1.Cms;
using ZstdSharp.Unsafe;
using Org.BouncyCastle.Tls.Crypto.Impl;
using Google.Apis.Calendar.v3.Data;


namespace HappyCode.NetCoreBoilerplate.Core.Services
{
    public class ForecastRequest
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int ForecastDays { get; set; } = 0;

        // public string[] Hourly { get; set; } = Array.Empty<string>();
        public string Hourly { get; set; }

        public string ToQueryString()
        {
            var HourlyParam = string.Join(",", Hourly);
            return $"?latitude={Latitude}&longitude={Longitude}&forecast_days={ForecastDays}&forecast_days=3&hourly=temperature_2m,relative_humidity_2m,rain,wind_speed_10m";
        }
    }

    public interface IWeatherService
    {
        public Task<HourlyForecastResponse?> GetForecastAsync(
            Double Latitude,
            Double Longitude,
            int ForecastDays,
            string[] Hourly
        );
    }

    public class WeatherService : IWeatherService
    {
        private readonly WeatherContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;

        private readonly IWeatherService _weatherService;


        public WeatherService(WeatherContext dbContext, HttpClient httpClient, ILogger<WeatherService> logger)
        {
            _dbContext = dbContext;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
            _logger = logger;
        }
    
        public List<WeatherDto> FromHourlyForecast (HourlyForecastResponse forecast)
        {
            var result = new List<WeatherDto>();

            if (forecast?.hourly.time == null) return result;
            int count = forecast.hourly.time.Count;

            for (int i = 0; i < count; i++)
            {
                var dto = new WeatherDto
                {
                    Time = DateTime.Parse(forecast.hourly.time[i]),
                    Temperature = forecast.hourly.temperature_2m?.ElementAtOrDefault(i) ?? 0,
                    Rain = forecast.hourly.rain?.ElementAtOrDefault(i) ?? 0,
                    WindSpeed = forecast.hourly.wind_speed_10m?.ElementAtOrDefault(i) ?? 0
                };
                result.Add(dto);
            }
            return result;
        }

       public WeatherPostDto? weatherAverage(Weather[] weatherList)
        {

            if (weatherList.Count() <= 0) { return null; }

            int n = weatherList.Count();
            double temperatureAverage = MathMethods.Truncate(weatherList.Select(x => x.Temperature)
                // .Aggregate(0, (double acc, double cur) => (acc + cur) / n), 4);
                .Aggregate(0, (double acc, double cur) => (acc + cur))/ n, 4);
            double windAverage = MathMethods.Truncate(weatherList.Select(x => x.WindSpeed)
                .Aggregate(0, (double acc, double cur) => (acc + cur))/ n, 4);
            double humidityAverage = MathMethods.Truncate(weatherList.Select(x => x.Rain)
            .Aggregate(0, (double acc, double cur) => (acc + cur))/ n, 4);

            return new WeatherPostDto
            {
                Time = weatherList[0].Time,
                Temperature = temperatureAverage,
                WindSpeed = windAverage,
                Rain = humidityAverage,
            };

        }


        // TODO: Normalize to receive two dates instead of startdate and hours quantity
        public List<WeatherDto> weatherRangeFromForecastResponse (DateTimeOffset datetime, Nullable<int> hourRange, HourlyForecastResponse forecast)
        {
            // get a hour before and after the target hour
            var weatherRange = new List<WeatherDto>();
            int n = forecast.hourly.time.Count();

            var dcurrent = forecast.hourly.time.FindIndex((x) => {
                return x == datetime.ToString("yyyy-MM-dd'T'HH:mm");
            });
            if (dcurrent < 0) { return []; }

            if (hourRange == null)
            {
                hourRange = (dcurrent <= 1) ? 1 : 3;
            }

            int startRange = dcurrent - ((int)hourRange / 2);
            int endRange = dcurrent + ((int)hourRange / 2);

            for (int i = startRange; i <= endRange; i++)
            {
                weatherRange.Add(new WeatherDto
                {
                    Time = DateTime.Parse(forecast.hourly.time[i]),
                    Rain = forecast.hourly.temperature_2m?.ElementAtOrDefault(i) ?? 0,
                    Temperature = forecast.hourly.temperature_2m?.ElementAtOrDefault(i) ?? 0,
                    WindSpeed = forecast.hourly.wind_speed_10m?.ElementAtOrDefault(i) ?? 0
                });
        }


            // Event[] eventList = res
            //     .Select(
            //         (res) => res.Start.Date == startDate.ToString("yyyy-MM-dd'T'HH:mm:ss-z")
            //     );



            return weatherRange;
        }


        public IWeather[] weatherListFromHourlyForecastResponse(HourlyForecastResponse hourly)
        {
            IWeather[] weatherList = [];
            for (int i = 0; i < hourly.hourly.time.Count; i++)
            {
                weatherList[i] = new()
                {
                    Time = DateTime.Parse(hourly.hourly.time[i]),
                    Rain = hourly.hourly.rain[i],
                    Temperature = hourly.hourly.wind_speed_10m[i],
                    WindSpeed = hourly.hourly.temperature_2m[i],
                };

                Console.Write(weatherList[i].ToString());
            }
            return weatherList;
        }

        public async Task<HourlyForecastResponse?> GetForecastAsync(
            Double Latitude,
            Double Longitude,
            int ForecastDays,
            string[] Hourly
            )

        {
            // var url = $"forecast?latitude={Latitude}&longitude={Longitude}&hourly=temperature_2m,rain,wind_speed_10m&current=temperature_2m,relative_humidity_2m,wind_speed_10m,wind_direction_10m";
            var url = $"forecast?latitude={Latitude}&longitude={Longitude}&hourly=temperature_2m,rain,wind_speed_10m&forecast_days={ForecastDays}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                _logger.LogDebug("Sending HTTP request {Method} {Uri}. .. ", request.Method, request.RequestUri);
                var response = await _httpClient.SendAsync(request);

                string body = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);

                var deserialized = JsonSerializer.Deserialize<HourlyForecastResponse>(body);
                return deserialized;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network/HTTP error while calling {Url}", url);
                throw; // or return null if you prefer
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON from {Url}", url);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed for {Url}: {Message}", url, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling {Url}", url);
                throw;
            }
        }
    }
}
