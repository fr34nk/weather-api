using HappyCode.NetCoreBoilerplate.Core.Dtos;
using HappyCode.NetCoreBoilerplate.Core.Models;

namespace HappyCode.NetCoreBoilerplate.Core.Extensions
{
    internal static class GoogleCalendarExtensions
    {
        public static GoogleCalendarEventDto MapToDto(this GoogleCalendarModel source)
        {
            return new GoogleCalendarEventDto 
            {
                id = source.id,
                data = source.data,
                eventId = source.eventId,
            };
        }
    }
}
