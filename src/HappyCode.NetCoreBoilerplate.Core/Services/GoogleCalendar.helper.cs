using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HappyCode.NetCoreBoilerplate.Core.Dtos;
using System;
using System.IO;
using System.Threading;

namespace HappyCode.NetCoreBoilerplate.Core.Services {

namespace HappyCode.NetCoreBoilerplate.Core.Services
{
    public class GoogleCalendarHelper
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "Google Calendar API .NET Integration";

        private CalendarService GetCalendarService()
        {
            UserCredential credential;

            string credentialsFile = Path.Combine(AppContext.BaseDirectory, "Settings", "credentials.json");

            using (var stream = new FileStream(credentialsFile, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public Event CreateEvent(CreateEventDto createEvent)
        {

            var service = GetCalendarService();

            Event newEvent = new Event()
            {
                Summary = createEvent.Title,
                Description = createEvent.Description,
                Start = new EventDateTime()
                {
                    DateTime = createEvent.StartTime,
                    TimeZone = createEvent.TimeZone
                },
                End = new EventDateTime()
                {
                    DateTime = createEvent.EndTime,
                    TimeZone = createEvent.TimeZone
                }
            };

            EventsResource.InsertRequest request = service.Events.Insert(newEvent, "primary");
            Event createdEvent = request.Execute();

            Console.WriteLine($"Event created: {createdEvent.HtmlLink}");
            return createdEvent;
        }

        public List<Event> GetEvents(int maxResults = 10)
        {
            var service = GetCalendarService();

            EventsResource.ListRequest request = service.Events.List("primary");
            

            request.TimeMin = DateTime.UtcNow;   // only future events
            request.ShowDeleted = false;
            request.SingleEvents = true;         // expand recurring events
            request.MaxResults = maxResults;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            return events.Items != null ? new List<Event>(events.Items) : new List<Event>();
        }


        public double hoursFromTimeRange (DateTimeOffset start, DateTimeOffset end) {
            return start > end
                ? start.Subtract(end).TotalHours
                : end.Subtract(start).TotalHours;
        }

        public List<Event> GetEventByDateAndHourRange(DateTimeOffset startDate,  DateTimeOffset endDate)
        {
            var service = GetCalendarService();
            EventsResource.ListRequest request = service.Events.List("primary");

            request.TimeMin = DateTime.UtcNow;   // only future events
            request.ShowDeleted = false;
            request.SingleEvents = true;         // expand recurring events
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            Events events = request.Execute();
            if (events.Items != null)
            {
                var res = new List<Event>(events.Items);
                return res
                    .FindAll((ev) =>
                    {
                        if (!String.IsNullOrEmpty(ev?.Start?.DateTimeRaw))
                        {
                            var evDateTime = DateTimeOffset.Parse(ev.Start.DateTimeRaw);
                            var targetDatetime = startDate;

                            var hourQtyRange = this.hoursFromTimeRange(startDate, endDate);

                            bool inRange = evDateTime >= startDate && evDateTime <= endDate;
                            return inRange;
                        }
                        else
                        {
                            return false;
                        }
                    });
            }
            else
            {
                return new List<Event>();
            }
        }
    }
}

