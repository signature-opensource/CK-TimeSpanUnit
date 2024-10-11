using System;

namespace CK.Core;

public static class DateTimeExtensions
{
    /// <summary>
    /// Adds a <see cref="WeakTimeSpan"/> to this DateTime.
    /// <para>
    /// If <see cref="WeakTimeSpan.IsValid"/> is false, this DateTime is returned unchanged.
    /// </para>
    /// </summary>
    /// <param name="dateTime">This DateTime.</param>
    /// <param name="span">The span to add.</param>
    /// <returns>The offsetted DateTime.</returns>
    public static DateTime Add( this DateTime dateTime, WeakTimeSpan span ) => DoAdd( dateTime, span.Unit, span.Count );

    /// <summary>
    /// Substract a <see cref="WeakTimeSpan"/> to this DateTime.
    /// <para>
    /// If <see cref="WeakTimeSpan.IsValid"/> is false, this DateTime is returned unchanged.
    /// </para>
    /// </summary>
    /// <param name="dateTime">This DateTime.</param>
    /// <param name="span">The span to add.</param>
    /// <returns>The offsetted DateTime.</returns>
    public static DateTime Substract( this DateTime dateTime, WeakTimeSpan span ) => DoAdd( dateTime, span.Unit, -span.Count );

    internal static DateTime DoAdd( DateTime dateTime, TimeSpanUnit unit, long count )
    {
        return unit switch
        {
            TimeSpanUnit.Year => dateTime.AddYears( (int)count ),
            TimeSpanUnit.Semester => dateTime.AddMonths( 6 * (int)count ),
            TimeSpanUnit.Quarter => dateTime.AddMonths( 3 * (int)count ),
            TimeSpanUnit.Month => dateTime.AddMonths( (int)count ),
            TimeSpanUnit.Day => dateTime.AddDays( count ),
            TimeSpanUnit.Hour => dateTime.AddHours( count ),
            TimeSpanUnit.Minute => dateTime.AddMinutes( count ),
            TimeSpanUnit.Second => dateTime.AddSeconds( count ),
            TimeSpanUnit.Millisecond => dateTime.AddMilliseconds( count ),
            _ => dateTime
        };
    }
}
