using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace CK.Core;

/// <summary>
/// Defines a <see cref="TimeSpan"/> in terms of <see cref="Count"/> <see cref="Unit"/>.
/// <para>
/// It is NOT (and cannot be converted to) an actual <see cref="TimeSpan"/>. 
/// </para>
/// </summary>
public readonly partial struct WeakTimeSpan : IMultiplyOperators<WeakTimeSpan,long,WeakTimeSpan>
{
    const ulong _countMask = (1UL << 56) - 1;
    readonly ulong _value;

    /// <summary>
    /// The maximal <see cref="Count"/> value.
    /// </summary>
    public const long MaxCount = long.MaxValue / TimeSpan.TicksPerMillisecond;

    /// <summary>
    /// Iitializes a new valid <see cref="WeakTimeSpan"/>.
    /// <para>
    /// The <c>default</c> value is the only invalid value.
    /// </para>
    /// </summary>
    /// <param name="unit">The time span unit. Must not be <see cref="TimeSpanUnit.None"/>.</param>
    /// <param name="count">The count of unit. Must be positive and not greater than <see cref="MaxCount"/>.</param>
    public WeakTimeSpan( TimeSpanUnit unit, long count = 1 )
    {
        Throw.CheckArgument( unit != TimeSpanUnit.None );
        Throw.CheckOutOfRangeArgument( count > 0 && count <= MaxCount );
        _value = ((ulong)unit) << 56 | (ulong)count;
    }

    public WeakTimeSpan( long count, TimeSpanUnit unit )
    {
        Throw.DebugAssert( unit != TimeSpanUnit.None );
        Throw.DebugAssert( count > 0 && count <= MaxCount );
        _value = ((ulong)unit) << 56 | (ulong)count;
    }

    /// <summary>
    /// Gets the count of unit. This is 0 or positive.
    /// </summary>
    public long Count => (long)(_value & _countMask);

    /// <summary>
    /// Gets the time span unit.
    /// </summary>
    public TimeSpanUnit Unit => (TimeSpanUnit)(_value >> 56);

    /// <summary>
    /// Gets whether this is a valid span.
    /// Only the <c>default</c> value is invalid.
    /// </summary>
    public bool IsValid => _value != 0;

    /// <summary>
    /// Returns a span for wich <see cref="Count"/> is multplied by <paramref name="factor"/>.
    /// <para>
    /// <paramref name="factor"/> must be positive (0 is invalid and will throw an <see cref="ArgumentException"/>).
    /// </para>
    /// </summary>
    /// <param name="factor">The greater than 0 factor.</param>
    /// <returns>The resulting span.</returns>
    public WeakTimeSpan Multiply( long factor ) => new WeakTimeSpan( Unit, Count *  factor );

    /// <summary>
    /// Calls <paramref name="left"/>.<see cref="Multiply(long)"/>. <paramref name="factor"/> must be positive.
    /// </summary>
    /// <param name="left">The span to multiply.</param>
    /// <param name="factor">The multipicative factor.</param>
    /// <returns>The resulting span.</returns>
    public static WeakTimeSpan operator *( WeakTimeSpan left, long factor ) => left.Multiply( factor );
    
    /// <summary>
    /// Tries to normalize this span to a more general one if possible: "Second:3600" returns "Hour:1" or
    /// "Quarter:8" returns "Year:2".
    /// <para>
    /// Days cannot be normalized to months, semesters or years.
    /// </para>
    /// </summary>
    /// <param name="ignoreSemester">True to normalize months or quarters directly into years: "Quarter:2" or "Month:6" will not be normalized.</param>
    /// <param name="ignoreQuarter">True to normalize months directly into semester or years.</param>
    /// <returns>This or a new span if it has been normalized.</returns>
    public WeakTimeSpan Normalize( bool ignoreSemester = false, bool ignoreQuarter = false )
    {
        long q, r, count = Count;
        switch( Unit )
        {
            case TimeSpanUnit.Millisecond:
                (q, r) = Math.DivRem( count, 1000 );
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Second ).Normalize();
                break;
            case TimeSpanUnit.Second:
                (q, r) = Math.DivRem( count, 60 );
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Minute ).Normalize();
                break;
            case TimeSpanUnit.Minute:
                (q, r) = Math.DivRem( count, 60 );
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Hour ).Normalize();
                break;
            case TimeSpanUnit.Hour:
                (q, r) = Math.DivRem( count, 24 );
                // Day ends here. It cannot be normalized to Month (nor Quarter, Semester or Year).
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Day );
                break;
            case TimeSpanUnit.Month:
                (q, r) = Math.DivRem( count, 12 );
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Year );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( count, 6 );
                    if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Semester );
                }
                if( !ignoreQuarter )
                {
                    (q, r) = Math.DivRem( count, 3 );
                    if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Quarter );
                }
                break;
            case TimeSpanUnit.Quarter:
                (q, r) = Math.DivRem( count, 4 );
                if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Year );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( count, 2 );
                    if( r == 0 ) return new WeakTimeSpan( q, TimeSpanUnit.Semester );
                }
                break;
            case TimeSpanUnit.Semester:
                if( (_value & 1) == 0 ) return new WeakTimeSpan( count >> 1, TimeSpanUnit.Year );
                break;
        }
        return this;
    }

    /// <summary>
    /// Computes the earliest date range for this span that contains the <paramref name="dateTime"/>.
    /// <para>
    /// The obtained range is not a <see cref="DateTimeRange"/>. This works for any span regardless
    /// of <see cref="IsEraligned"/>and it is not necessarily the same as the <see cref="GetDateTimeRange(DateTime, bool)"/>
    /// when IsEraligned is true.
    /// </para>
    /// </summary>
    /// <param name="dateTime">The instant to frame.</param>
    /// <returns>The range start and end (exluded).</returns>
    public (DateTime Start, DateTime End) GetEarliestRange( DateTime dateTime )
    {
        TimeSpanUnit unit = Unit;
        var start = unit.GetStart( dateTime, 1 - Count );
        var end = unit.GetStart( dateTime, 1 );
        Throw.DebugAssert( DateTimeExtensions.DoAdd( start, unit, Count ) == end );
        return (start, end);
    }

    /// <summary>
    /// Computes the date range for this span that centered on the <paramref name="dateTime"/>.
    /// <para>
    /// The obtained range is not a <see cref="DateTimeRange"/>. This works for any span regardless
    /// of <see cref="IsEraligned"/>and it is not necessarily the same as the <see cref="GetDateTimeRange(DateTime, bool)"/>
    /// when IsEraligned is true.
    /// </para>
    /// </summary>
    /// <param name="dateTime">The instant to frame.</param>
    /// <returns>The range start and end (exluded).</returns>
    public (DateTime Start, DateTime End) GetCenteredRange( DateTime dateTime )
    {
        TimeSpanUnit unit = Unit;
        long count = Count;
        var start = unit.GetStart( dateTime, 1 - count );
        var end = unit.GetStart( dateTime, 1 );
        count >>= 1;
        start = DateTimeExtensions.DoAdd( start, unit, count );
        end = DateTimeExtensions.DoAdd( end, unit, count );
        Throw.DebugAssert( DateTimeExtensions.DoAdd( start, unit, Count ) == end );
        return (start, end);
    }

    /// <summary>
    /// Computes the latest date range for this span that contains the <paramref name="dateTime"/>.
    /// <para>
    /// The obtained range is not a <see cref="DateTimeRange"/>. This works for any span regardless
    /// of <see cref="IsEraligned"/> and it is not necessarily the same as the <see cref="GetDateTimeRange(DateTime, bool)"/>
    /// when IsEraligned is true.
    /// </para>
    /// </summary>
    /// <param name="dateTime">The instant to frame.</param>
    /// <returns>The range start and end (exluded).</returns>
    public (DateTime Start, DateTime End) GetLatestRange( DateTime dateTime )
    {
        TimeSpanUnit unit = Unit;
        var start = unit.GetStart( dateTime, 0 );
        var end = unit.GetStart( dateTime, Count );
        Throw.DebugAssert( DateTimeExtensions.DoAdd( start, unit, Count ) == end );
        return (start, end);
    }

    /// <summary>
    /// Gets whether this time span is aligned.
    /// </summary>
    public bool IsEraligned => GetEraligned( out var _, out var _ );

    bool GetEraligned( out TimeSpanUnit unit, out long count )
    {
        // Normalizing is a side effect.
        unit = Unit;
        count = Count;
        if( count == 1 ) return true;
        switch( unit )
        {
            case TimeSpanUnit.Year:
                return true;
            case TimeSpanUnit.Semester:
                if( (count & 1) == 0 )
                {
                    unit = TimeSpanUnit.Year;
                    count = count >> 1;
                    return true;
                }
                return false;
            case TimeSpanUnit.Quarter:
                if( count == 2 )
                {
                    unit = TimeSpanUnit.Semester;
                    count = 1;
                    return true;
                }
                if( (count & 3) == 0 )
                {
                    unit = TimeSpanUnit.Year;
                    count = count >> 2;
                    return true;
                }
                return false;
            case TimeSpanUnit.Month:
                if( count == 2 || count == 4 ) return true;
                if( count == 3 )
                {
                    unit = TimeSpanUnit.Quarter;
                    count = 1;
                    return true;
                }
                if( count == 6 )
                {
                    unit = TimeSpanUnit.Semester;
                    count = 1;
                    return true;
                }
                if( count < 12 ) return false;
                unit = TimeSpanUnit.Year;
                (count, var monthRem) = Math.DivRem( count, 12 );
                return monthRem == 0;
            case TimeSpanUnit.Day:
                return true;
            case TimeSpanUnit.Hour:
                return IsAlignedHour( ref unit, ref count );
            case TimeSpanUnit.Minute:
                return IsAlignedMinute( ref unit, ref count );
            case TimeSpanUnit.Second:
                return IsAlignedSecond( ref unit, ref count );
            case TimeSpanUnit.Millisecond:
                return IsAlignedMillisecond( ref unit, ref count );
        }
        return false;

        static bool IsAlignedHour( ref TimeSpanUnit unit, ref long count )
        {
            if( count < 24 ) return (24 % count) == 0;
            (count, var r) = Math.DivRem( count, 24 );
            unit = TimeSpanUnit.Day;
            return r == 0;
        }

        static bool IsAlignedMinute( ref TimeSpanUnit unit, ref long count )
        {
            if( count < 60 ) return (60 % count) == 0;
            unit = TimeSpanUnit.Hour;
            (count, var r) = Math.DivRem( count, 60 );
            return r == 0
                    ? count == 1
                        ? true
                        : IsAlignedHour( ref unit, ref count )
                    : false;
        }

        static bool IsAlignedSecond( ref TimeSpanUnit unit, ref long count )
        {
            if( count < 60 ) return (60 % count) == 0;
            unit = TimeSpanUnit.Minute;
            (count, var r) = Math.DivRem( count, 60 );
            return r == 0
                    ? count == 1
                        ? true
                        : IsAlignedMinute( ref unit, ref count )
                    : false;
        }

        static bool IsAlignedMillisecond( ref TimeSpanUnit unit, ref long count )
        {
            if( count < 1000 ) return (1000 % count) == 0;
            unit = TimeSpanUnit.Second;
            (count, var r) = Math.DivRem( count, 1000 );
            return r == 0
                    ? count == 1
                        ? true
                        : IsAlignedSecond( ref unit, ref count )
                    : false;
        }
    }

    /// <summary>
    /// Gets the start date of this <see cref="IsEraligned"/> span for any <paramref name="dateTime"/>.
    /// <para><see cref="IsEraligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="dateTime">The DateTime for which the start of the aligned range must be computed.</param>
    /// <returns>The start of the aligned range.</returns>
    public DateTime GetEralignedStart( DateTime dateTime ) => GetEralignedStart( dateTime, out var _, out var _ );

    /// <summary>
    /// Gets the start date of this <see cref="IsEraligned"/> span for any <paramref name="dateTime"/>.
    /// <para><see cref="IsEraligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="dateTime">The DateTime for which the start of the aligned range must be computed.</param>
    /// <param name="normalizedUnit">Outputs the normalized unit that may differ from this <see cref="Unit"/></param>
    /// <param name="normalizedCount">Outputs the normalized count that may differ from this <see cref="Count"/></param>
    /// <returns>The start of the aligned range.</returns>
    public DateTime GetEralignedStart( DateTime dateTime, out TimeSpanUnit normalizedUnit, out long normalizedCount )
    {
        CheckEraligned( out normalizedUnit, out normalizedCount );
        return DoGetEralignedStart( dateTime, normalizedUnit, normalizedCount );
    }

    void CheckEraligned( out TimeSpanUnit normalizedUnit, out long normalizedCount )
    {
        if( !GetEraligned( out normalizedUnit, out normalizedCount ) )
        {
            Throw.InvalidOperationException( $"The WeakTimeSpan '{ToString()}' is not eraligned." );
        }
    }

    static DateTime DoGetEralignedStart( DateTime dateTime, TimeSpanUnit normalizedUnit, long normalizedCount )
    {
        switch( normalizedUnit )
        {
            case TimeSpanUnit.Year:
                var y = (int)normalizedCount;
                return new DateTime( 1 + ((dateTime.Year - 1) / y) * y, 1, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Semester:
                Throw.DebugAssert( normalizedCount == 1 );
                return new DateTime( dateTime.Year, dateTime.Month > 6 ? 7 : 1, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Quarter:
                Throw.DebugAssert( normalizedCount == 1 );
                return new DateTime( dateTime.Year, 1 + ((dateTime.Month - 1) / 3) * 3, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Month:
                Throw.DebugAssert( normalizedCount is 1 or 2 or 4 );
                var mCount = (int)normalizedCount;
                return new DateTime( dateTime.Year, 1 + ((dateTime.Month - 1) / mCount) * mCount, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Day:
                return RoundTicks( dateTime, normalizedCount * TimeSpan.TicksPerDay );
            case TimeSpanUnit.Hour:
                Throw.DebugAssert( normalizedCount is 1 or 2 or 3 or 4 or 6 or 8 or 12 );
                return RoundTicks( dateTime, normalizedCount * TimeSpan.TicksPerHour );
            case TimeSpanUnit.Minute:
                Throw.DebugAssert( normalizedCount is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                return RoundTicks( dateTime, normalizedCount * TimeSpan.TicksPerMinute );
            case TimeSpanUnit.Second:
                Throw.DebugAssert( normalizedCount is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                return RoundTicks( dateTime, normalizedCount * TimeSpan.TicksPerSecond );
            default:
                Throw.DebugAssert( normalizedUnit == TimeSpanUnit.Millisecond );
                Throw.DebugAssert( normalizedCount is 1 or 2 or 4 or 5 or 8 or 10 or 20 or 25 or 40 or 50 or 100 or 125 or 200 or 250 or 500 );
                return RoundTicks( dateTime, normalizedCount * TimeSpan.TicksPerMillisecond );
        }

        static DateTime RoundTicks( DateTime dateTime, long step ) => new DateTime( (dateTime.Ticks / step) * step, dateTime.Kind );
    }

    /// <summary>
    /// Gets the <see cref="DateTimeRange"/> of this <see cref="IsEraligned"/> span for any <paramref name="dateTime"/>.
    /// <para><see cref="IsEraligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="dateTime">The DateTime for which the <see cref="DateTimeRange"/> must be computed.</param>
    /// <param name="normalize">
    /// False to keep this span (like "Quarter:8") for the <see cref="DateTimeRange.Span"/>.
    /// By default, the span is normalized (i.e. "Year:2").
    /// </param>
    /// <returns>The range.</returns>
    public DateTimeRange GetDateTimeRange( DateTime dateTime, bool normalize = true )
    {
        return normalize
                ? new DateTimeRange( GetEralignedStart( dateTime, out var u, out var c ), new WeakTimeSpan( c, u ) )
                : new DateTimeRange( GetEralignedStart( dateTime ), this );
    }

    /// <summary>
    /// Gets the <see cref="DateTimeRange"/> of this <see cref="IsEraligned"/> span by its <see cref="DateTimeRange.Index"/>.
    /// <para><see cref="IsEraligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="index">The index of the range. Must be 0 or positive.</param>
    /// <param name="kind">The kind of the <see cref="DateTimeRange.Start"/>.</param>
    /// <returns>The <see cref="DateTimeRange"/> at this <paramref name="index"/>.</returns>
    public DateTimeRange GetDateTimeRange( long index, DateTimeKind kind = DateTimeKind.Utc )
    {
        Throw.CheckArgument( index >= 0 );
        CheckEraligned( out var unit, out var count );

        return new DateTimeRange( GetStart( unit, count, index, kind ), new WeakTimeSpan( count, unit ) );

        static DateTime GetStart( TimeSpanUnit unit, long count, long index, DateTimeKind kind )
        {
            switch( unit )
            {
                case TimeSpanUnit.Year:
                    return new DateTime( 1 + (int)(index * count), 1, 1, 0, 0, 0, kind );
                case TimeSpanUnit.Semester:
                    Throw.DebugAssert( count == 1 );
                    return new DateTime( 1 + ((int)index >> 1), (index & 1) == 0 ? 1 : 7, 1, 0, 0, 0, kind );
                case TimeSpanUnit.Quarter:
                    Throw.DebugAssert( count == 1 );
                    return new DateTime( 1 + ((int)index >> 2), 1 + ((int)index & 3) * 3, 1, 0, 0, 0, kind );
                case TimeSpanUnit.Month:
                    Throw.DebugAssert( count is 1 or 2 or 4 );
                    var (y,m) = Math.DivRem( index * count, 12 );
                    return new DateTime( (int)y + 1, (int)m + 1, 1, 0, 0, 0, kind );
                case TimeSpanUnit.Day:
                    return new DateTime( index * count * TimeSpan.TicksPerDay, kind );
                case TimeSpanUnit.Hour:
                    Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 6 or 8 or 12 );
                    return new DateTime( index * count * TimeSpan.TicksPerHour, kind );
                case TimeSpanUnit.Minute:
                    Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                    return new DateTime( index * count * TimeSpan.TicksPerMinute, kind );
                case TimeSpanUnit.Second:
                    Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                    return new DateTime( index * count * TimeSpan.TicksPerSecond, kind );
                default:
                    Throw.DebugAssert( unit == TimeSpanUnit.Millisecond );
                    Throw.DebugAssert( count is 1 or 2 or 4 or 5 or 8 or 10 or 20 or 25 or 40 or 50 or 100 or 125 or 200 or 250 or 500 );
                    return new DateTime( index * count * TimeSpan.TicksPerMillisecond, kind );
            }
        }
    }

    /// <summary>
    /// Gets the "Unit:Count" representation of this span.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() => $"{Unit.ToStringFast()}:{Count}";

    public void Deconstruct( out TimeSpanUnit unit, out long count )
    {
        unit = Unit;
        count = Count;
    }

}
