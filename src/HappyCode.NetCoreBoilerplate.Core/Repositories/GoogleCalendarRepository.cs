using System.Threading;
using System.Threading.Tasks;
using HappyCode.NetCoreBoilerplate.Core.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Timers;
using Google;
using Mysqlx;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Misc;


namespace HappyCode.NetCoreBoilerplate.Core.Repositories
{
    public interface IGoogleCalendarRepository
    {
        Task<GoogleCalendarEventPostDto> InsertEventAsync(GoogleCalendarEventPostDto weatherPostDto, CancellationToken cancellationToken);
        Task<GoogleCalendarEventPostDto> getEventAsync(string evId, CancellationToken cancellationToken);
        Task<GoogleCalendarEventPostDto[]> getStoredEventListAsync(CancellationToken cancellationToken);
    }

    internal class GoogleCalendarRepository : RepositoryBase<GoogleCalendarModel, GoogleCalendarContext>, IGoogleCalendarRepository
    {
        public GoogleCalendarRepository(GoogleCalendarContext dbContext, ILogger<WeatherRepository> logger)
            : base(dbContext)
        { }

        public async Task<GoogleCalendarEventPostDto> InsertEventAsync (GoogleCalendarEventPostDto calendarEvent, CancellationToken cancellationToken)
        {
            string json = JsonConvert.SerializeObject(calendarEvent);
            var calendarModel = new GoogleCalendarModel
            {
                eventId = calendarEvent.ETag,
                data = json
            };

            await DbContext.GoogleCalendarModel.AddAsync(calendarModel, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);

            return calendarEvent;
        }


        public async Task<List<GoogleCalendarEventPostDto>> getStoredLAsync()
        {
            
            List<GoogleCalendarEventPostDto> events = new();
            GoogleCalendarEventDto[] results = [];
            foreach (var item in results)
            {

            var parsedJsonObject = JsonConvert.DeserializeObject<GoogleCalendarEventPostDto>(
                item.data,
                new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

                GoogleCalendarEventPostDto dto = new GoogleCalendarEventPostDto
                {
                    Created = parsedJsonObject.Created,
                    Creator = parsedJsonObject.Creator,
                    Attendees = parsedJsonObject.Attendees,
                    ColorId = parsedJsonObject.ColorId,
                    Attachments = parsedJsonObject.Attachments,
                    Description = parsedJsonObject.Description,
                    End = parsedJsonObject.End,
                    Start = parsedJsonObject.Start,
                    Status = parsedJsonObject.Status,
                    Id = parsedJsonObject.Id,
                    ETag = parsedJsonObject.ETag,
                    ConferenceData = parsedJsonObject.ColorId
                };

                events.Add(dto);
            }
            return [];
        }



        public async Task<GoogleCalendarEventPostDto[]> getStoredEventListAsync (CancellationToken cancellationToken)
        {

            var calendarResult = await DbContext.GoogleCalendarModel
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return calendarResult
                .Select(
                    item =>
                {
                    var parsed = JsonConvert.DeserializeObject<GoogleCalendarEventPostDto>(
                        item.data,
                        new JsonSerializerSettings
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore
                        });

                    return new GoogleCalendarEventPostDto
                    {
                        Created = parsed.Created,
                        Creator = parsed.Creator,
                        Attendees = parsed.Attendees,
                        ColorId = parsed.ColorId,
                        Attachments = parsed.Attachments,
                        Description = parsed.Description,
                        End = parsed.End,
                        Start = parsed.Start,
                        Status = parsed.Status,
                        Id = parsed.Id,
                        ETag = parsed.ETag,
                        ConferenceData = parsed.ColorId
                    };
                }).ToArray();
        }



        public async Task<GoogleCalendarEventPostDto> getEventAsync (string evId, CancellationToken cancellationToken)
        {
            var ev = await DbContext
                .GoogleCalendarModel
                // .Select(el => el.eventId == evId)
                .FirstAsync();

            try
            {
                // dynamic calendarEvent = JsonConvert.DeserializeObject(File.ReadAllText("file.json"));
                dynamic calendarEvent = JsonConvert.DeserializeObject(ev.data);
                return calendarEvent;
            }
            catch (Exception ex)
            {
                throw new Exception("[getEventListAsync] Parsing Json Error: ", ex);
            }
        }
    }
}
