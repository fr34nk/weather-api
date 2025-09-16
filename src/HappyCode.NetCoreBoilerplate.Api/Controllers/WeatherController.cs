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
using Microsoft.AspNetCore.Builder.Extensions;
using NetTopologySuite.Mathematics;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.IO;


namespace HappyCode.NetCoreBoilerplate.Api.Controllers
{
    [FeatureGate(FeatureFlags.DockerCompose)]
    [Route("api/weather")]
    public class WeatherController : ApiControllerBase
    {

        private readonly ILogger<GoogleCalendarController> _logger;
        private readonly IWeatherRepository _weatherRepository;
        private readonly IFeatureManager _featureManager;
        private readonly WeatherService _weatherService;
        private readonly GoogleCalendarHelper _calendarService;

        public WeatherController(
            ILogger<GoogleCalendarController> logger,
            WeatherService weatherservice,
            IWeatherRepository weatherRepository,
            IFeatureManager featureManager,
            GoogleCalendarHelper googleCalendarHelper
        )
        {
            _logger = logger;
            _weatherService = weatherservice;
            _weatherRepository = weatherRepository;
            _featureManager = featureManager;
            _calendarService = googleCalendarHelper;
        }

        [HttpGet("weather")]
        [ProducesResponseType(typeof(IEnumerable<WeatherDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _weatherRepository.GetAllAsync(cancellationToken);
            return Ok(result);
        }


        [HttpPost("next_rainy_events")]
        // [ProducesResponseType(typeof(IEnumerable<WeatherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<Weather>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRainyEventsAsync(
            [FromBody] GetRainyEventsRequestDto body,
            CancellationToken cancellationToken = default)
        {
            var parsedEndDate = DateTimeOffset.Parse(body.endDate);
            var parsedStartDate = DateTimeOffset.Parse(body.startDate);

            var eventsByDate = _calendarService.GetEventByDateAndHourRange(parsedStartDate, parsedEndDate);

            WeatherPostDto weatherAverage;
            if (eventsByDate.Count() > 0) {

                Weather[] weatherList = [];
                // var rangeHours = _calendarService.hoursFromTimeRange(parsedStartDate, parsedEndDate);
                // var days = rangeHours > 24 ? int.CreateTruncating(rangeHours / 24) : 1;

                for (int _ievent = 0; _ievent < eventsByDate.Count(); _ievent++) {
                    var currentEvent = eventsByDate[_ievent];

                    // TODO need to check periodic events
                    var parsedEventStartDate = DateTimeOffset.Parse(currentEvent.Start.DateTimeRaw);
                    var parsedEventEndDate = DateTimeOffset.Parse(currentEvent.End.DateTimeRaw);

                    // Is day in range of forecast api ? calculate it : return (cant forecast)  
                    //  - [16 days for api current being used]

                    var curDateTime = DateTimeOffset.Now;
                    if (curDateTime.AddDays(16) < parsedEventEndDate)
                        return Ok(String.Format("Couldnt forecast the event cause of limitation of days of forecast api. End time event date {enddate}", parsedEventEndDate));

                    var neededForecastDays = Math.Ceiling(parsedEventEndDate.Subtract(curDateTime).TotalDays);

                    // Apend at least 2 hours before and 1 hour before and after ;

                    // var days = eventRangeHours > 24 ? int.CreateTruncating(eventRangeHours / 24) : 1;
                    var forecastResult = await _weatherService.GetForecastAsync(
                        body.Latitude,
                        body.Longitude,
                        // days,
                        int.CreateTruncating(neededForecastDays),
                        ["temperature_2m", "relative_humidity_2m", "wind_speed_10m"]
                    );

                    var eventRangeHours = _calendarService.hoursFromTimeRange(
                        parsedEventStartDate.AddHours(-1),
                        parsedEventEndDate.AddHours(1)
                    );

                    var result = _weatherService.weatherRangeFromForecastResponse(
                        parsedStartDate,
                        int.CreateTruncating(eventRangeHours),
                        forecastResult
                    );

                    weatherList = result
                        .Select(res => {
                            return new Weather
                            {
                                Time = res.Time,
                                Temperature = res.Temperature,
                                Rain = res.Rain,
                                WindSpeed = res.WindSpeed
                            };
                        })
                        .ToArray();

                    weatherAverage = _weatherService.weatherAverage(weatherList);
                    return Ok(weatherAverage);
                }
                return Ok(weatherList);
            }

            var msg = String.Format("No event found in this date rage {0} {1}", parsedStartDate, parsedEndDate);
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>> msg <<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine(msg.ToString());
            return Ok(msg);
        }


        [HttpPost("average")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWeatherAverageAsync(
            [FromBody] GetRainyEventsRequestDto body,
            CancellationToken cancellationToken = default
        )
        {
            var weatherForecastList = await _weatherService.GetForecastAndFilterByDateRangeAsync(
                body.Latitude,
                body.Longitude,
                DateTimeOffset.Parse(body.startDate),
                DateTimeOffset.Parse(body.endDate),
                ["temperature_2m", "relative_humidity_2m", "wind_speed_10m"]
            );

            Weather[] modelArray = weatherForecastList 
                .Select(dto => new Weather
                {
                    Time = dto.Time,
                    Temperature = dto.Temperature,
                    Rain = dto.Rain,
                    WindSpeed = dto.WindSpeed
                })
                .ToArray();

            var average = _weatherService.weatherAverage(modelArray);
            if (average != null)
            {
            _logger.LogDebug("Average: {Average}", average.ToString());
            Weather insertedWeather = await _weatherRepository.InsertAsync(average, cancellationToken);
            return Ok(insertedWeather);
            }
            else
            {
                return Ok(String.Format("Could not get forecast for this time range: {0} {1}", body.startDate.ToString(), body.endDate.ToString()));
            }
        }

        /**
         * forecast - 
        **/
        [HttpPost("forecast")]
        [ProducesResponseType(typeof(IEnumerable<WeatherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWeatherForecastAsync(
            // [FromBody] GetWeatherReqDto forecastFields,
            [FromBody] GetRainyEventsRequestDto body,
            CancellationToken cancellationToken = default
        )
        {
            var list = await _weatherService.GetForecastAndFilterByDateRangeAsync(
                body.Latitude,
                body.Longitude,
                DateTimeOffset.Parse(body.startDate),
                DateTimeOffset.Parse(body.endDate),
                ["temperature_2m", "relative_humidity_2m", "wind_speed_10m"]
            );

            return Ok(list);
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
