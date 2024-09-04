using System;
using System.Text;

namespace CK.Core;

/// <summary>
/// Models an absolute time range. This can only be obtained by <see cref="WeakTimeSpan.GetDateTimeRange(DateTime)"/>
/// when the <see cref="WeakTimeSpan.IsAligned"/> is true.
/// </summary>
public readonly struct DateTimeRange
{
    readonly DateTime _start;
    readonly WeakTimeSpan _span;

    internal DateTimeRange( DateTime start, WeakTimeSpan span )
    {
        _start = start;
        _span = span;
    }

    /// <summary>
    /// Gets the start of this range.
    /// </summary.
    public DateTime Start => _start;

    /// <summary>
    /// Gets the description of this range.
    /// </summary>
    public WeakTimeSpan Span => _span;

    /// <summary>
    /// Gets the end of this range. This end is excluded (it is the start of the next range).
    /// </summary>
    public DateTime End => _span.Unit.GetStart( _start, _span.Count );

    /// <summary>
    /// Gets the index of this <see cref="DateTimeRange"/>.
    /// </summary>
    public long Index
    {
        get
        {
            var (u, c) = _span;
            return u switch
            {
                TimeSpanUnit.Year => (_start.Year - 1) / c,
                // For Semester and Quarter, c == 1 (normalized to Semester or Year).
                TimeSpanUnit.Semester => ((_start.Year - 1) << 1) + (_start.Month > 6 ? 1 : 0),
                TimeSpanUnit.Quarter => ((_start.Year - 1) << 2) + (_start.Month - 1) / 3,
                TimeSpanUnit.Month => ((_start.Year - 1) * 12 + _start.Month - 1) / c,
                TimeSpanUnit.Day => (_start.Ticks / TimeSpan.TicksPerDay) / c,
                TimeSpanUnit.Hour => (_start.Ticks / TimeSpan.TicksPerHour) / c,
                TimeSpanUnit.Minute => (_start.Ticks / TimeSpan.TicksPerMinute) / c,
                TimeSpanUnit.Second => (_start.Ticks / TimeSpan.TicksPerSecond) / c,
                TimeSpanUnit.Millisecond => (_start.Ticks / TimeSpan.TicksPerMillisecond) / c,
                _ => Throw.CKException<long>( "Unreachable" )
            };
        }
    }

    /// <summary>
    /// Overridden to return the "[start, end[".
    /// </summary>
    /// <returns>A readable string.</returns>
    public override string ToString()
    {
        var b = new StringBuilder();
        var u = _span.Unit;
        b.Append( '[' );
        TimeSpanUnitPathPart.None.WritePath( b, _start, u );
        b.Append( ", " );
        TimeSpanUnitPathPart.None.WritePath( b, End, u );
        b.Append( '[' );
        return b.ToString();
    }
}
