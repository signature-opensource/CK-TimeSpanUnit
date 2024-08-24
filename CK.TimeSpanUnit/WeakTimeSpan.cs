using System;

namespace CK.Core;

/// <summary>
/// Defines a <see cref="TimeSpan"/> in terms of <see cref="Count"/> <see cref="Unit"/>.
/// <para>
/// It is NOT (and cannot be converted to) an actual <see cref="TimeSpan"/>. 
/// </para>
/// </summary>
public readonly struct WeakTimeSpan : ISpanParsable<WeakTimeSpan>
{
    readonly int _count;
    readonly TimeSpanUnit _unit;

    /// <summary>
    /// Iitializes a new valid <see cref="WeakTimeSpan"/>.
    /// <para>
    /// The <c>default</c> value is the only invalid value.
    /// </para>
    /// </summary>
    /// <param name="unit">The time span unit. Must not be <see cref="TimeSpanUnit.None"/>.</param>
    /// <param name="count">The count of unit. Must be positive.</param>
    public WeakTimeSpan( TimeSpanUnit unit, int count = 1 )
    {
        Throw.CheckArgument( unit != TimeSpanUnit.None );
        Throw.CheckOutOfRangeArgument( count > 0 );
        _unit = unit;
        _count = count;
    }

    /// <summary>
    /// Gets the count of unit. This is 0 or positive.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Gets the time span unit.
    /// </summary>
    public TimeSpanUnit Unit => _unit;

    /// <summary>
    /// Gets whether this is a valid span.
    /// Only the <c>default</c> value is invalid.
    /// </summary>
    public bool IsValid => _count != 0;

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
        int q;
        int r;
        switch( _unit )
        {
            case TimeSpanUnit.Millisecond:
                (q, r) = Math.DivRem( _count, 1000 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Second, q ).Normalize();
                break;
            case TimeSpanUnit.Second:
                (q, r) = Math.DivRem( _count, 60 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Minute, q ).Normalize();
                break;
            case TimeSpanUnit.Minute:
                (q, r) = Math.DivRem( _count, 60 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Hour, q ).Normalize();
                break;
            case TimeSpanUnit.Hour:
                (q, r) = Math.DivRem( _count, 24 );
                // Day ends here. It cannot be normalized to Month (nor Quarter, Semester or Year).
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Day, q );
                break;
            case TimeSpanUnit.Month:
                (q, r) = Math.DivRem( _count, 12 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, q );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( _count, 6 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Semester, q );

                }
                if( !ignoreQuarter )
                {
                    (q, r) = Math.DivRem( _count, 3 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Quarter, q );
                }
                break;
            case TimeSpanUnit.Quarter:
                (q, r) = Math.DivRem( _count, 4 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, q );
                if( !ignoreSemester )
                {
                    (q, r) = Math.DivRem( _count, 2 );
                    if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Semester, q );
                }
                break;
            case TimeSpanUnit.Semester:
                (q, r) = Math.DivRem( _count, 2 );
                if( r == 0 ) return new WeakTimeSpan( TimeSpanUnit.Year, q );
                break;
        }
        return this;
    }

    /// <summary>
    /// Gets the "Unit:Count" representation of this span.
    /// </summary>
    /// <returns>The string representation.</returns>
    public override string ToString() => $"{_unit.ToStringFast()}:{_count}";

    /// <inheritdoc />
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: the returned value <see cref="WeakTimeSpan.IsValid"/> CAN BE false.
    /// </remarks>
    public static WeakTimeSpan Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => TryParse( s, provider, out var result ) ? result : Throw.FormatException<WeakTimeSpan>();

    /// <summary>
    /// Parses a span into a value.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: the returned value <see cref="WeakTimeSpan.IsValid"/> CAN BE false.
    /// </remarks>
    /// <returns>The result of parsing s.</returns>
    public static WeakTimeSpan Parse( ReadOnlySpan<char> s ) => Parse( s, null );

    /// <inheritdoc />
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: the returned value <see cref="WeakTimeSpan.IsValid"/> CAN BE false.
    /// </remarks>
    public static WeakTimeSpan Parse( string s, IFormatProvider? provider ) => Parse( s.AsSpan(), provider );

    /// <summary>
    /// Tries to parse <paramref name="s"/> into a value.
    /// </summary>
    /// <param name="s">The characters to parse.</param>
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: <paramref name="result"/> can be NOT <see cref="WeakTimeSpan.IsValid"/>
    /// even if this returns true.
    /// </remarks>
    /// <returns>true if s was successfully parsed; otherwise, false.</returns>
    public static bool TryParse( string? s, out WeakTimeSpan result ) => TryParse( s.AsSpan(), null, out result );

    /// <inheritdoc />
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: <paramref name="result"/> can be NOT <see cref="WeakTimeSpan.IsValid"/>
    /// even if this returns true.
    /// </remarks>
    public static bool TryParse( string? s, IFormatProvider? provider, out WeakTimeSpan result ) => TryParse( s.AsSpan(), provider, out result );

    /// <inheritdoc cref="TryParse(string?, out WeakTimeSpan)" />
    public static bool TryParse( ReadOnlySpan<char> s, out WeakTimeSpan result ) => TryParse( s, null, out result );

    /// <inheritdoc />
    /// <remarks>
    /// "None" (or even "None:0" or "None:xxx") is allowed: <paramref name="result"/> can be NOT <see cref="WeakTimeSpan.IsValid"/>
    /// even if this returns true.
    /// </remarks>
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out WeakTimeSpan result )
    {
        if( s.TryMatchTimeSpanUnit( out var unit, StringComparison.OrdinalIgnoreCase ) )
        {
            s.SkipWhiteSpaces();
            if( unit == TimeSpanUnit.None )
            {
                result = default;
                // Allow "None" to not have :count (but it must be at least zero or positive: :- is not in this grammar).
                if( s.TryMatch( ':' ) )
                {
                    s.SkipWhiteSpaces();
                    return s.Length == 0 || (s.TryMatchInt32( out var ignored ) && ignored >= 0);
                }
                return true;
            }
            if( s.TryMatch( ':' ) && s.SkipWhiteSpaces() && s.TryMatchInt32( out var count, minValue: 1 ) )
            {
                result = new WeakTimeSpan( unit, count );
                return true;
            }
        }
        result = default;
        return false;
    }

}
