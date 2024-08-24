using System;

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
