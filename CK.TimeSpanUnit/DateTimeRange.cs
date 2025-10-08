using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace CK.Core;

/// <summary>
/// Models an absolute time range. This can only be obtained by <see cref="WeakTimeSpan.GetDateTimeRange(DateTime)"/>
/// or by <see cref="WeakTimeSpan.GetDateTimeRange(long, DateTimeKind)"/> when the <see cref="WeakTimeSpan.IsEraligned"/> is true.
/// <para>
/// This is a 128 bits value type that contains the <see cref="Start"/> and the <see cref="Span"/>.
/// <see cref="Index"/> and <see cref="End"/> are computed properties.
/// </para>
/// <para>
/// A <c>default</c> value of this type is the only invalid value of this type.
/// </para>
/// </summary>
public readonly struct DateTimeRange : IComparable<DateTimeRange>,
                                       IComparisonOperators<DateTimeRange, DateTimeRange, bool>,
                                       IEquatable<DateTimeRange>,
                                       IEqualityOperators<DateTimeRange, DateTimeRange, bool>
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
    /// Compares this time range with the other one. <see cref="Span"/> comes first, then <see cref="Start"/>
    /// is used.
    /// </summary>
    /// <param name="other">The other time range.</param>
    /// <returns>Standard comparison value. See <see cref="IComparable{T}.CompareTo(T?)"/>.</returns>
    public int CompareTo( DateTimeRange other )
    {
        int cmp = Span.CompareTo( other.Span );
        return cmp == 0 ? Start.CompareTo( other.Start ) : cmp;
    }

    /// <summary>
    /// Strict equality (<see cref="Span"/> and <see cref="Start"/> must be the same).
    /// </summary>
    /// <param name="other">The other time span.</param>
    /// <returns>True if this span is equal to the other one; otherwise, false.</returns>
    public bool Equals( DateTimeRange other ) => Span == other.Span && Start == other.Start;

    public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is DateTimeRange other && Equals( other );

    public override int GetHashCode() => HashCode.Combine( Span, Start );

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
    /// Get a next range. This throws if the resulting range cannot be computed.
    /// </summary>
    /// <param name="offset">Positive number of ranges to shift forward.</param>
    /// <returns>The previous range.</returns>
    public DateTimeRange GetNext( long offset = 1 )
    {
        Throw.CheckArgument( offset >= 0 );
        if( offset == 0 ) return this;
        return _span.GetDateTimeRange( Index + offset );
    }

    /// <summary>
    /// Get a previous range. This never overflows: the very first range is returned (in year 1).
    /// </summary>
    /// <param name="offset">Positive number of ranges to shift backward.</param>
    /// <returns>The previous range.</returns>
    public DateTimeRange GetPrevious( long offset = 1 )
    {
        Throw.CheckArgument( offset >= 0 );
        if( offset == 0 ) return this;
        long i = Math.Max( Index - offset, 0 );
        return _span.GetDateTimeRange( i );
    }

    /// <summary>
    /// Overridden to return the "[start,end[".
    /// </summary>
    /// <returns>A readable string.</returns>
    public override string ToString() => ToString( _span.Unit, _start, End );


    /// <summary>
    /// Helper that writes a date range "[start,end[".
    /// </summary>
    /// <param name="unit">The precision to use.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <returns>A readable string.</returns>
    public static string ToString( TimeSpanUnit unit, DateTime start, DateTime end )
    {
        return WriteRange( new StringBuilder(), unit, start, end ).ToString();
    }

    /// <summary>
    /// Helper that writes a date range "[start,end[".
    /// </summary>
    /// <param name="b">The builder to use.</param>
    /// <param name="unit">The precision to use.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <returns>The builder.</returns>
    public static StringBuilder WriteRange( StringBuilder b, TimeSpanUnit unit, DateTime start, DateTime end )
    {
        b.Append( '[' );
        TimeSpanUnitPathPart.None.WritePath( b, start, unit );
        b.Append( ',' );
        TimeSpanUnitPathPart.None.WritePath( b, end, unit );
        b.Append( '[' );
        return b;
    }

    public static bool operator ==( DateTimeRange left, DateTimeRange right ) => left.Equals( right );

    public static bool operator !=( DateTimeRange left, DateTimeRange right ) => !(left == right);

    public static bool operator <( DateTimeRange left, DateTimeRange right ) => left.CompareTo( right ) < 0;

    public static bool operator <=( DateTimeRange left, DateTimeRange right ) => left.CompareTo( right ) <= 0;

    public static bool operator >( DateTimeRange left, DateTimeRange right ) => left.CompareTo( right ) > 0;

    public static bool operator >=( DateTimeRange left, DateTimeRange right ) => left.CompareTo( right ) >= 0;
}
