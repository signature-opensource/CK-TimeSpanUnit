using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace CK.Core;

/// <summary>
/// Defines a <see cref="TimeSpan"/> in terms of <see cref="Count"/> <see cref="Unit"/>.
/// <para>
/// It is NOT (and cannot be converted to) an actual <see cref="TimeSpan"/>. 
/// </para>
/// </summary>
public readonly partial struct WeakTimeSpan 
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
    /// Tries to normalize this span to a more general one if possible: "Second:3600" returns "Minute:1" or
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
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Second, q ).Normalize();
                break;
            case TimeSpanUnit.Second:
                (q, r) = Math.DivRem( count, 60 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Minute, q ).Normalize();
                break;
            case TimeSpanUnit.Minute:
                (q, r) = Math.DivRem( count, 60 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Hour, q ).Normalize();
                break;
            case TimeSpanUnit.Hour:
                (q, r) = Math.DivRem( count, 24 );
                // Day ends here. It cannot be normalized to Month (nor Quarter, Semester or Year).
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Day, q );
                break;
            case TimeSpanUnit.Month:
                (q, r) = Math.DivRem( count, 12 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, q );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( count, 6 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Semester, q );

                }
                if( !ignoreQuarter )
                {
                    (q, r) = Math.DivRem( count, 3 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Quarter, q );
                }
                break;
            case TimeSpanUnit.Quarter:
                (q, r) = Math.DivRem( count, 4 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, q );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( count, 2 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Semester, q );
                }
                break;
            case TimeSpanUnit.Semester:
                if( (_value & 1) == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, count >> 1 );
                break;
        }
        return this;
    }

    /// <summary>
    /// Gets whether this time span is aligned.
    /// </summary>
    public bool IsAligned => GetAligned( out var _, out var _ );

    bool GetAligned( out TimeSpanUnit nUnit, out long nCount )
    {
        // Normalizing is a side effect.
        nUnit = TimeSpanUnit.None;
        nCount = 0;

        long c = Count;
        if( c == 1 ) return true;
        switch( Unit )
        {
            case TimeSpanUnit.Year:
                return true;
            case TimeSpanUnit.Semester:
                nUnit = TimeSpanUnit.Year;
                nCount = c >> 1;
                return (c & 1) == 0;
            case TimeSpanUnit.Quarter:
                if( c == 2 )
                {
                    nUnit = TimeSpanUnit.Semester;
                    nCount = 1;
                    return true;
                }
                nUnit = TimeSpanUnit.Year;
                nCount = c >> 2;
                return (c & 3) == 0;
            case TimeSpanUnit.Month:
                if( c == 2 || c == 4 ) return true;
                if( c == 3 )
                {
                    nUnit = TimeSpanUnit.Quarter;
                    nCount = 1;
                    return true;
                }
                if( c == 6 )
                {
                    nUnit = TimeSpanUnit.Semester;
                    nCount = 1;
                    return true;
                }
                if( c < 12 ) return false;
                nUnit = TimeSpanUnit.Year;
                (nCount, var monthRem) = Math.DivRem( c, 12 );
                return monthRem == 0;
            case TimeSpanUnit.Day:
                return true;
            case TimeSpanUnit.Hour:
                return IsAlignedHour( c, ref nUnit, ref nCount );
            case TimeSpanUnit.Minute:
                return IsAlignedMinute( c, ref nUnit, ref nCount );
            case TimeSpanUnit.Second:
                return IsAlignedSecond( c, ref nUnit, ref nCount );
            case TimeSpanUnit.Millisecond:
                return IsAlignedMillisecond( c, ref nUnit, ref nCount );
        }
        return false;

        static bool IsAlignedHour( long hour, ref TimeSpanUnit nUnit, ref long nCount )
        {
            (nCount, var r) = Math.DivRem( hour, 24 );
            if( nCount != 0 )
            {
                nUnit = TimeSpanUnit.Day;
                return r == 0;
            }
            Throw.DebugAssert( r > 0 && r < 24 );
            return (24 % r) == 0;
        }

        static bool IsAlignedMinute( long minute, ref TimeSpanUnit nUnit, ref long nCount )
        {
            if( minute < 60 ) return (60 % minute) == 0;
            nUnit = TimeSpanUnit.Hour;
            (nCount, var r) = Math.DivRem( minute, 60 );
            return r == 0
                    ? nCount == 1
                        ? true
                        : IsAlignedHour( nCount, ref nUnit, ref nCount )
                    : false;
        }

        static bool IsAlignedSecond( long second, ref TimeSpanUnit nUnit, ref long nCount )
        {
            if( second < 60 ) return (60 % second) == 0;
            nUnit = TimeSpanUnit.Minute;
            (nCount, var r) = Math.DivRem( second, 60 );
            return r == 0
                    ? nCount == 1
                        ? true
                        : IsAlignedMinute( nCount, ref nUnit, ref nCount )
                    : false;
        }

        static bool IsAlignedMillisecond( long millisecond, ref TimeSpanUnit nUnit, ref long nCount )
        {
            if( millisecond < 1000 ) return (1000 % millisecond) == 0;
            nUnit = TimeSpanUnit.Second;
            (nCount, var r) = Math.DivRem( millisecond, 1000 );
            return r == 0
                    ? nCount == 1
                        ? true
                        : IsAlignedSecond( nCount, ref nUnit, ref nCount )
                    : false;
        }
    }

    /// <summary>
    /// Gets the start date of this <see cref="IsAligned"/> span for any <paramref name="dateTime"/>.
    /// <para><see cref="IsAligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="dateTime">The DateTime for which the start of the aligned range must be computed.</param>
    /// <returns>The start of the aligned range.</returns>
    public DateTime GetAlignedStart( DateTime dateTime )
    {
        if( !GetAligned( out var unit, out var count ) )
        {
            Throw.InvalidOperationException( $"The WeakTimeSpan '{ToString()}' is not aligned." );
        }
        if( unit == TimeSpanUnit.None )
        {
            unit = Unit;
            count = Count;
        }
        switch( unit )
        {
            case TimeSpanUnit.Year:
                var y = (int)count;
                return new DateTime( 1 + ((dateTime.Year - 1) / y) * y, 1, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Semester:
                Throw.DebugAssert( count == 1 );
                return new DateTime( dateTime.Year, dateTime.Month > 6 ? 7 : 1, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Quarter:
                Throw.DebugAssert( count == 1 );
                return new DateTime( dateTime.Year, 1 + ((dateTime.Month - 1) / 3), 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Month:
                Throw.DebugAssert( count is 1 or 2 or 4 );
                var mCount = (int)count;
                return new DateTime( dateTime.Year, 1 + ((dateTime.Month - 1) / mCount) * mCount, 1, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Day:
                return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Hour:
                Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 6 or 8 or 12 );
                var mHour = (int)count;
                return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, (dateTime.Hour / mHour) * mHour, 0, 0, dateTime.Kind );
            case TimeSpanUnit.Minute:
                Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                var mMinute = (int)count;
                return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, (dateTime.Minute / mMinute) * mMinute, 0, dateTime.Kind );
            case TimeSpanUnit.Second:
                Throw.DebugAssert( count is 1 or 2 or 3 or 4 or 5 or 6 or 10 or 12 or 15 or 20 or 30 );
                var mSecond = (int)count;
                return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, (dateTime.Second / mSecond) * mSecond, dateTime.Kind );
            default:
                Throw.DebugAssert( unit == TimeSpanUnit.Millisecond );
                Throw.DebugAssert( count is 1 or 2 or 4 or 5 or 8 or 10 or 20 or 25 or 40 or 50 or 100 or 125 or 200 or 250 or 500 );
                var mMS = (int)count;
                return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, (dateTime.Millisecond / mMS) * mMS, dateTime.Kind );
        }
    }

    /// <summary>
    /// Gets the <see cref="DateTimeRange"/> of this <see cref="IsAligned"/> span for any <paramref name="dateTime"/>.
    /// <para><see cref="IsAligned"/> must be true otherwise a <see cref="InvalidOperationException"/> is thrown.</para>
    /// </summary>
    /// <param name="dateTime">The DateTime for which the <see cref="DateTimeRange"/> must be computed.</param>
    /// <returns>The range.</returns>
    public DateTimeRange GetDateTimeRange( DateTime dateTime ) => new DateTimeRange( GetAlignedStart( dateTime ), this );

    /// <summary>
    /// Gets the <see cref="DateTimeRange"/> of this <see cref="IsAligned"/> span by its <see cref="DateTimeRange.Index"/>.
    /// </summary>
    /// <param name="index">The index of the range. Must be 0 or positive.</param>
    /// <param name="kind">The kind of the <see cref="DateTimeRange.Start"/>.</param>
    /// <returns>The <see cref="DateTimeRange"/> at this <paramref name="index"/>.</returns>
    public DateTimeRange GetDateTimeRange( long index, DateTimeKind kind = DateTimeKind.Utc )
    {
        Throw.CheckArgument( index >= 0 );
        if( !GetAligned( out var unit, out var count ) )
        {
            Throw.InvalidOperationException( $"The WeakTimeSpan '{ToString()}' is not aligned." );
        }
        if( unit == TimeSpanUnit.None )
        {
            unit = Unit;
            count = Count;
        }

        return new DateTimeRange( GetStart( unit, count, index, kind ), new WeakTimeSpan( unit, count ) );

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
