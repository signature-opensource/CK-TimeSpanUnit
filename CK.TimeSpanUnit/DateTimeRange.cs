using System;
using System.Text;

namespace CK.Core;

/// <summary>
/// Models an absolute time range. This can only be obtained by <see cref="WeakTimeSpan.GetDateTimeRange(DateTime)"/>
/// when the <see cref="WeakTimeSpan.IsEraligned"/> is true.
/// <para>
/// This is a 128 bits value type that contains the <see cref="Start"/> and the <see cref="Span"/>.
/// <see cref="Index"/> and <see cref="End"/> are computed properties.
/// </para>
/// <para>
/// A <c>default</c> value of this type is invalid.
/// </para>
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
    /// Gets whether this is a valid range.
    /// Only the <c>default</c> value is invalid.
    /// </summary>
    public bool IsValid => _span.IsValid;

    /// <summary>
    /// Gets the start of this range.
    /// </summary>
    public DateTime Start => _start;

    /// <summary>
    /// Gets the description of this range.
    /// </summary>
    public WeakTimeSpan Span => _span;

    /// <summary>
    /// Gets the end of this range. This end is excluded (it is the start of the next range).
    /// <para>
    /// This property is not stored, it is recomputed from the <see cref="Span"/> and <see cref="Start"/>.
    /// </para>
    /// </summary>
    public DateTime End => _span.Unit.GetStart( _start, _span.Count );

    /// <summary>
    /// Gets whether a <paramref name="dateTime"/> is in this range.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>Whether the DateTime is contained in this range.</returns>
    public bool Contains( DateTime dateTime ) => Start <= dateTime && dateTime < End;

    /// <summary>
    /// Gets whether another range is the same or is contained in this range.
    /// </summary>
    /// <param name="other">The DateTime.</param>
    /// <returns>Whether the DateTime is contained in this range.</returns>
    public bool Contains( DateTimeRange other ) => Start <= other.Start && other.End <= End;

    /// <summary>
    /// Gets the index of this <see cref="DateTimeRange"/>.
    /// <para>
    /// This property is not stored, it is recomputed from the <see cref="Span"/> and <see cref="Start"/>.
    /// </para>
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
    /// Overridden to return the "[start,end[".
    /// </summary>
    /// <returns>A readable string.</returns>
    public override string ToString()
    {
        var b = new StringBuilder();
        var u = _span.Unit;
        b.Append( '[' );
        TimeSpanUnitPathPart.None.WritePath( b, _start, u );
        b.Append( ',' );
        TimeSpanUnitPathPart.None.WritePath( b, End, u );
        b.Append( '[' );
        return b.ToString();
    }
}
