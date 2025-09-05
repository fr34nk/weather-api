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
using Org.BouncyCastle.Asn1.Misc;
using Microsoft.AspNetCore.Components.Web;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Protocol;
using MinimalApis.Extensions.Binding;

namespace HappyCode.NetCoreBoilerplate.Api.Controllers
{
    [FeatureGate(FeatureFlags.DockerCompose)]
    [Route("api/calendar")]
    [ApiController]
    public class GoogleCalendarController : ApiControllerBase
    {
        private readonly ILogger<GoogleCalendarController> _logger;
        private readonly IGoogleCalendarRepository _calendarRepository;
        private readonly IFeatureManager _featureManager;
        private readonly GoogleCalendarHelper _calendarService;

        public GoogleCalendarController(
            ILogger<GoogleCalendarController> logger,
            IGoogleCalendarRepository calendarRepository,
            IFeatureManager featureManager,
            WeatherService weatherService,
            GoogleCalendarHelper calendarService
        ) : base()
        {
            _logger = logger;
            _calendarService = calendarService;
            _calendarRepository = calendarRepository;
            _featureManager = featureManager;
        }


        [HttpGet("google_calendar/events")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCalendarEventsAsync(
            CancellationToken cancellationToken
        )
        {
            var evs =  _calendarService.GetEvents(10); 
            return Ok(evs);
        }


        [HttpGet("events")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNextCalendarEventsAsync (
            // [FromQuery] string rain,
            CancellationToken cancellationToken
        )
        {
            var ev = await _calendarRepository.getStoredEventListAsync(cancellationToken);
            return Ok(ev);
        }


        [HttpPost("events")]
        [ProducesResponseType(typeof(GoogleCalendarEventPostDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync(
            [FromBody] GoogleCalendarEventPostDto calendarEventPostDto,
            CancellationToken cancellationToken = default)
        {
            var result = await _calendarRepository.InsertEventAsync(calendarEventPostDto, cancellationToken);
            Response.Headers.Append("x-date-created", DateTime.UtcNow.ToString("s"));
            return CreatedAtAction("Get", new { id = result }, result);
        }

        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(WeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(HttpValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOneAsync(
            [FromRoute] string eventId,
            CancellationToken cancellationToken = default)
        {

            Console.Write(">>>>>>>>>>>>>>>>>>>>>> eventId <<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.Write(eventId.ToString());
            var result = await _calendarRepository.getEventAsync(eventId, cancellationToken);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }



        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var result = await _calendarRepository.getEventAsync(id, cancellationToken);

            if (result.ToString() != null)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
