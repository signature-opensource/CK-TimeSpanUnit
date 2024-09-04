using System;
using System.Globalization;

namespace CK.Core;

public readonly partial struct WeakTimeSpan : ISpanParsable<WeakTimeSpan>
{
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
    /// <param name="result">The parsed result.</param>
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
            if( s.TryMatch( ':' )
                && s.SkipWhiteSpaces()
                && long.TryParse( s, CultureInfo.InvariantCulture, out long count )
                && count > 0 )
            {
                result = new WeakTimeSpan( unit, count );
                return true;
            }
        }
        result = default;
        return false;
    }


}
