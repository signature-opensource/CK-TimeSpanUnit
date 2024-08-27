using System;
using System.Runtime.CompilerServices;

namespace CK.Core;

/// <summary>
/// Extends <see cref="TimeSpanUnit"/> and provides <see cref="TryMatchTimeSpanUnit(ref ReadOnlySpan{char}, out TimeSpanUnit, StringComparison)"/>.
/// </summary>
public static class TimeSpanUnitExtensions
{
    /// <summary>
    /// Fast ToString implementation.
    /// </summary>
    /// <param name="value">This value.</param>
    /// <returns>The enum name.</returns>
    public static string ToStringFast( this TimeSpanUnit value )
            => value switch
            {
                TimeSpanUnit.Year => nameof( TimeSpanUnit.Year ),
                TimeSpanUnit.Semester => nameof( TimeSpanUnit.Semester ),
                TimeSpanUnit.Quarter => nameof( TimeSpanUnit.Quarter ),
                TimeSpanUnit.Month => nameof( TimeSpanUnit.Month ),
                TimeSpanUnit.Day => nameof( TimeSpanUnit.Day ),
                TimeSpanUnit.Hour => nameof( TimeSpanUnit.Hour ),
                TimeSpanUnit.Minute => nameof( TimeSpanUnit.Minute ),
                TimeSpanUnit.Second => nameof( TimeSpanUnit.Second ),
                TimeSpanUnit.Millisecond => nameof( TimeSpanUnit.Millisecond ),
                _ => nameof( TimeSpanUnit.None ),
            };

    /// <summary>
    /// Computes the <see cref="WeakTimeSpan"/> in this unit between 2 dates.
    /// </summary>
    /// <param name="unit">This unit.</param>
    /// <param name="t1">One of the date.</param>
    /// <param name="t2">The other date.</param>
    /// <returns>The weak time span between the dates.</returns>
    public static WeakTimeSpan GetWeakTimeSpan( this TimeSpanUnit unit, DateTime t1, DateTime t2 )
    {
        long delta = GetDeltaUnit( unit, t1, t2 );
        return new WeakTimeSpan( unit, Math.Abs( delta ) + 1 );
    }

    /// <summary>
    /// Gets whether the 2 dates are in this same unit.
    /// </summary>
    /// <param name="unit">This unit.</param>
    /// <param name="t1">One of the date.</param>
    /// <param name="t2">The other date.</param>
    /// <returns>Whether the dates are in the same unit.</returns>
    public static bool SameWeakTimeSpan( this TimeSpanUnit unit, DateTime t1, DateTime t2 ) => GetDeltaUnit( unit, t1, t2 ) == 0;

    static long GetDeltaUnit( TimeSpanUnit unit, DateTime t1, DateTime t2 )
    {
        return unit switch
        {
            TimeSpanUnit.Year => t1.Year - t2.Year,
            TimeSpanUnit.Semester => SemesterCount( t1 ) - SemesterCount( t2 ),
            TimeSpanUnit.Quarter => QuarterCount( t1 ) - QuarterCount( t2 ),
            TimeSpanUnit.Month => MonthCount( t1 ) - MonthCount( t2 ),
            TimeSpanUnit.Day => DayCount( t1 ) - DayCount( t2 ),
            TimeSpanUnit.Hour => HourCount( t1 ) - HourCount( t2 ),
            TimeSpanUnit.Minute => MinuteCount( t1 ) - MinuteCount( t2 ),
            TimeSpanUnit.Second => SecondCount( t1 ) - SecondCount( t2 ),
            TimeSpanUnit.Millisecond => MillisecondCount( t1 ) - MillisecondCount( t2 ),
            _ => Throw.ArgumentOutOfRangeException<long>( nameof( unit ) )
        };
    }
    static int SemesterCount( DateTime t ) => (t.Year << 1) + (t.Month > 6 ? 1 : 0);
    static int QuarterCount( DateTime t ) => (t.Year << 2) + ((t.Month-1) / 3);
    static int MonthCount( DateTime t ) => t.Year * 12 + t.Month;
    static long DayCount( DateTime t ) => t.Ticks / TimeSpan.TicksPerDay;
    static long HourCount( DateTime t ) => t.Ticks / TimeSpan.TicksPerHour;
    static long MinuteCount( DateTime t ) => t.Ticks / TimeSpan.TicksPerMinute;
    static long SecondCount( DateTime t ) => t.Ticks / TimeSpan.TicksPerSecond;
    static long MillisecondCount( DateTime t ) => t.Ticks / TimeSpan.TicksPerMillisecond;

    /// <summary>
    /// Tries to match one of the enum name and forward the <paramref name="head"/> on success.
    /// "None" is valid at this level.
    /// </summary>
    /// <param name="head">This head head.</param>
    /// <param name="value">The parsed value.</param>
    /// <param name="comparison">How to compare.</param>
    /// <returns>True on success, false otherwise.</returns>
    public static bool TryMatchTimeSpanUnit( this ref ReadOnlySpan<char> head, out TimeSpanUnit value, StringComparison comparison = StringComparison.OrdinalIgnoreCase )
    {
        if( head.TryMatch( nameof( TimeSpanUnit.Year ) ) )
        {
            value = TimeSpanUnit.Year;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Semester ), comparison ) )
        {
            value = TimeSpanUnit.Semester;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Quarter ), comparison ) )
        {
            value = TimeSpanUnit.Quarter;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Month ), comparison ) )
        {
            value = TimeSpanUnit.Month;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Day ), comparison ) )
        {
            value = TimeSpanUnit.Day;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Hour ), comparison ) )
        {
            value = TimeSpanUnit.Hour;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Minute ), comparison ) )
        {
            value = TimeSpanUnit.Minute;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Second ), comparison ) )
        {
            value = TimeSpanUnit.Second;
        }
        else if( head.TryMatch( nameof( TimeSpanUnit.Millisecond ), comparison ) )
        {
            value = TimeSpanUnit.Millisecond;
        }
        else 
        {
            value = TimeSpanUnit.None;
            if( !head.TryMatch( nameof( TimeSpanUnit.None ), comparison ) ) return false;
        }
        return true;
    }
}
