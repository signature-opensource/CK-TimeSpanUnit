using System.Globalization;

namespace CK.Core;

/// <summary>
/// Defines the units of time span.
/// </summary>
/// <remarks>
/// Weeks are not easy (see https://en.wikipedia.org/wiki/ISO_week_date) and cannot really be
/// grouped by semester or quarter. If this need to be supported, the ISO definition implemented
/// by <see cref="ISOWeek"/> should be used (week starts on monday).
/// <para>
/// One should also decide how the Day of Week should be rendered: this may be the number (1 to 7),
/// the <see cref="CultureInfo.InvariantCulture"/>'s <see cref="CultureInfo.DateTimeFormat"/>'s
/// <see cref="DateTimeFormatInfo.DayNames"/> ("Monday" to "Sunday"), <see cref="DateTimeFormatInfo.AbbreviatedDayNames"/>
/// ("Mon" to "Sun") or <see cref="DateTimeFormatInfo.ShortestDayNames"/> ("Mo" to "Su").
/// These names array start with sunday (the en-US way) instead of the ISO monday. This will have to be handled
/// to conform to the ISO rules.
/// </para>
/// <para>
/// Supporting weeks may be for the future.
/// </para>
/// </remarks>
public enum TimeSpanUnit : byte
{
    /// <summary>
    /// Non applicable.
    /// </summary>
    None = 0,
    Year,
    Semester,
    Quarter,
    Month,
    Day,
    Hour,
    Minute,
    Second,
    Millisecond
}
