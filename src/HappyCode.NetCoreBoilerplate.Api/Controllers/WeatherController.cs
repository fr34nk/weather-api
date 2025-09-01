using HappyCode.NetCoreBoilerplate.Core;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using HappyCode.NetCoreBoilerplate.Core.Services;
using HappyCode.NetCoreBoilerplate.Core.Models;
using System.Linq;
using Microsoft.Extensions.Logging;



namespace HappyCode.NetCoreBoilerplate.Api.Controllers
{
    [FeatureGate(FeatureFlags.DockerCompose)]
    [Route("api/weather")]
    public class WeatherController : ApiControllerBase
    {

        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherRepository _weatherRepository;
        private readonly IFeatureManager _featureManager;
        private readonly WeatherService _weatherService;

        public WeatherController(
            ILogger<WeatherController> logger,
            WeatherService weatherservice,
            IWeatherRepository weatherRepository,
            IFeatureManager featureManager
        )
        {
            _logger = logger;
            _weatherService = weatherservice;
            _weatherRepository = weatherRepository;
            _featureManager = featureManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpPost("weather/average")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWeatherAsync(
            GetForecastDaysDto forecastFields,
            CancellationToken cancellationToken = default
        )
        {
            var forecastResult = await _weatherService.GetForecastAsync(
                forecastFields.Latitude,
                forecastFields.Longitude,
                forecastFields.PastDays,
                ["temperature_2m", "relative_humidity_2m", "wind_speed_10m"]
            );

            List<WeatherDto> weatherList = _weatherService.FromHourlyForecast(forecastResult);
            Weather[] modelArray = weatherList
                .Select(dto => new Weather
                {
                    Time = dto.Time,
                    Temperature = dto.Temperature,
                    Humidity = dto.Humidity,
                    WindSpeed = dto.WindSpeed
                })
                .ToArray();

            var average = _weatherService.weatherAverage(modelArray);
            _logger.LogDebug("Average: {Average}", average.ToString());

            Weather insertedWeather = await _weatherRepository.InsertAsync(average, cancellationToken);
            return Ok(insertedWeather);
        }


        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(WeatherDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (result == null) { return NotFound(); }

            return Ok(result);
        }

        [HttpGet("oldest")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOldestAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.GetOldestAsync(cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] WeatherPutDto weatherPutDto,
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.UpdateAsync(id, weatherPutDto, cancellationToken);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(
            [FromBody] WeatherPostDto weatherPostDto,
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.InsertAsync(weatherPostDto, cancellationToken);
            Response.Headers.Append("x-date-created", DateTime.UtcNow.ToString("s"));
            return CreatedAtAction("Get", new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.DeleteByIdAsync(id, cancellationToken);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
