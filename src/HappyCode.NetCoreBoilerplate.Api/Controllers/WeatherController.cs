using HappyCode.NetCoreBoilerplate.Core;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using HappyCode.NetCoreBoilerplate.Core.Services;

namespace HappyCode.NetCoreBoilerplate.Api.Controllers
{
    [FeatureGate(FeatureFlags.DockerCompose)]
    [Route("api/weather")]
    public class WeatherController : ApiControllerBase
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherservice, IWeatherRepository weatherRepository, IFeatureManager featureManager)
        {

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

        // [HttpGet("{id}")]
        [HttpGet("getfore")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var result = await _weatherService.GetForecastAsync(
                13.41, 52.52, 10, ["temperature_2m", "relative_humidity_2m", "wind_speed_10m"]
            );

            Console.Write("sdlfkj");
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(WeatherDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.GetByIdWithDetailsAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("oldest")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOldestAsync(
            CancellationToken cancellationToken = default)
        {
            if (await _featureManager.IsEnabledAsync(FeatureFlags.ConnectionInfo.ToString()))
            {

                return Ok(new WeatherDto
                {
                    Temperature = 12,
                    Humidity = 13,
                    WindSpeed = 14,
                    Time = new DateTime(2025, 12, 12, 12, 12, 12)
                });
            }

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

            // var result = await _weatherRepository.InsertAsync(weatherPostDto, cancellationToken);
            // Response.Headers.Append("x-date-created", DateTime.UtcNow.ToString("s"));
            // return CreatedAtAction("Get", new { id = result.Id }, result);


            Console.Write("insert async ==========");

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
