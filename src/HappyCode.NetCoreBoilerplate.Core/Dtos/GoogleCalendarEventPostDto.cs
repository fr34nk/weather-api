using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;


namespace HappyCode.NetCoreBoilerplate.Core.Dtos
{
    public class Creator
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Id { get; set; }
        public bool? Self { get; set; }
    }

    public class Organizer
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Id { get; set; }
        public bool? Self { get; set; }
    }

    public class DateTimeInfo
    {
        public string? Date { get; set; }
        public string? DateTimeRaw { get; set; }
        public DateTimeOffset? DateTimeDateTimeOffset { get; set; }
        public DateTimeOffset? DateTime { get; set; }
        public string? TimeZone { get; set; }
        public string? ETag { get; set; }
    }

    public class Reminders
    {
        public List<object>? Overrides { get; set; }
        public bool? UseDefault { get; set; }
    }

    public class GoogleCalendarEventPostDto
    {
        public bool? AnyoneCanAddSelf { get; set; }
        public object? Attachments { get; set; }
        public object? Attendees { get; set; }
        public bool? AttendeesOmitted { get; set; }
        public object? BirthdayProperties { get; set; }
        public string? ColorId { get; set; }
        public object? ConferenceData { get; set; }

        public DateTimeOffset? CreatedRaw { get; set; }
        public DateTimeOffset? CreatedDateTimeOffset { get; set; }
        public DateTimeOffset? Created { get; set; }

        public Creator? Creator { get; set; }
        public string? Description { get; set; }
        public DateTimeInfo? End { get; set; }
        public bool? EndTimeUnspecified { get; set; }
        public string? ETag { get; set; }
        public string? EventType { get; set; }
        public object? ExtendedProperties { get; set; }
        public object? FocusTimeProperties { get; set; }
        public object? Gadget { get; set; }
        public bool? GuestsCanInviteOthers { get; set; }
        public bool? GuestsCanModify { get; set; }
        public bool? GuestsCanSeeOtherGuests { get; set; }
        public string? HangoutLink { get; set; }
        public string? HtmlLink { get; set; }
        public string? ICalUID { get; set; }
        public string? Id { get; set; }
        public string? Kind { get; set; }
        public string? Location { get; set; }
        public bool? Locked { get; set; }
        public Organizer? Organizer { get; set; }
        public object? OriginalStartTime { get; set; }
        public object? OutOfOfficeProperties { get; set; }
        public bool? PrivateCopy { get; set; }
        public object? Recurrence { get; set; }
        public string? RecurringEventId { get; set; }
        public Reminders? Reminders { get; set; }
        public int? Sequence { get; set; }
        public object? Source { get; set; }
        public DateTimeInfo Start { get; set; }
        public string? Status { get; set; }
        public string? Summary { get; set; }
        public string? Transparency { get; set; }

        public DateTimeOffset? UpdatedRaw { get; set; }
        public DateTimeOffset? UpdatedDateTimeOffset { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string? Visibility { get; set; }
        public object? WorkingLocationProperties { get; set; }
    }
}
