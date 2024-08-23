using System;
using System.Text;

namespace CK.Core;

/// <summary>
/// Extends <see cref="TimeSpanUnitPathPart"/> enumeration.
/// </summary>
public static class TimeSpanUnitPathPartExtensions
{
    /// <summary>
    /// Gets the <paramref name="instant"/> as a path up to the <see cref="TimeSpanUnit"/> part.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <param name="instant">The <see cref="DateTime"/> that must be obtained as a path.</param>
    /// <param name="upTo">The most precise unit to obtain. Cannot be <see cref="TimeSpanUnit.None"/>.</param>
    /// <returns>The <see cref="DateTime"/> expressed as a path.</returns>
    public static string GetPath( this TimeSpanUnitPathPart parts, DateTime instant, TimeSpanUnit upTo )
    {
        Throw.CheckArgument( upTo != TimeSpanUnit.None );
        Throw.DebugAssert( upTo >= TimeSpanUnit.Year );
        StringBuilder b = new StringBuilder();
        WritePath( parts, b, instant, upTo );
        return b.ToString();
    }

    /// <summary>
    /// Writes the <paramref name="instant"/> as a path up to the <see cref="TimeSpanUnit"/> part.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <param name="b">The target string builder.</param>
    /// <param name="instant">The <see cref="DateTime"/> that must be obtained as a path.</param>
    /// <param name="upTo">The most precise unit to obtain. Cannot be <see cref="TimeSpanUnit.None"/>.</param>
    /// <param name="hourInlineSeparator">Inline separator to use beween date and time component.</param>
    /// <param name="millisecondInlineSeparator">Inline separator to use beween seconds and milliseconds.</param>
    public static void WritePath( this TimeSpanUnitPathPart parts,
                                  StringBuilder b,
                                  DateTime instant,
                                  TimeSpanUnit upTo,
                                  char hourInlineSeparator = 'T',
                                  char millisecondInlineSeparator = '.' )
    {
        // Year is always here.
        var y = instant.Year;
        if( y < 10 ) b.Append( "000" );
        else if( y < 100 ) b.Append( "00" );
        else if( y < 1000 ) b.Append( '0' );
        b.Append( y );
        if( upTo > TimeSpanUnit.Year )
        {
            // If we must stop to Semester or Quarter, it's easy.
            if( upTo <= TimeSpanUnit.Quarter )
            {
                // Semester must appear if we want it or if we want up to the Quarter
                // and this path has the Semester or InlineSemeter.
                if( upTo == TimeSpanUnit.Semester || (parts & (TimeSpanUnitPathPart.Semester|TimeSpanUnitPathPart.InlineSemester)) != 0 )
                {
                    b.Append( parts.HasSemester() ? '/' : '-' );
                    b.Append( 'S' ).Append( instant.Month <= 6 ? '1' : '2' );
                }
                // Quarter must only appear if we want it.
                if( upTo == TimeSpanUnit.Quarter )
                {
                    b.Append( parts.HasQuarter() ? '/' : '-' );
                    b.Append( 'Q' ).Append( 1 + (instant.Month - 1) / 3 );
                }
            }
            else
            {
                Throw.DebugAssert( upTo >= TimeSpanUnit.Month );
                // Starting from month, it's a little bit more complicated.
                // If the month is not required in the path (HasMonth is false) and
                // day is required (HasHay is true), we introduce the Day of Year (1 to 366)
                // with the D prefix. This is typically used with HasQuarter or HasSemester
                // but can be used as-is.
                // In any other configuration, we follow the standard pattern.
                // This Month/Day game doesn't change anything for HasSemester and HasQuarter.
                // Note that having both HasSemester and HasQuarter is rather useless (but who knows).
                if( (parts & (TimeSpanUnitPathPart.Semester | TimeSpanUnitPathPart.InlineSemester)) != 0 )
                {
                    b.Append( parts.HasSemester() ? '/' : '-' );
                    b.Append( 'S' ).Append( instant.Month <= 6 ? '1' : '2'  );
                }
                if( (parts & (TimeSpanUnitPathPart.Quarter | TimeSpanUnitPathPart.InlineQuarter)) != 0 )
                {
                    b.Append( parts.HasQuarter() ? '/' : '-' );
                    b.Append( 'Q' ).Append( 1 + (instant.Month - 1) / 3 );
                }
                if( upTo > TimeSpanUnit.Month && !parts.HasMonth() && parts.HasDay() )
                {
                    // Day of Year.
                    b.Append( "/D" );
                    Append000( b, instant.DayOfYear );
                    if( upTo > TimeSpanUnit.Day )
                    {
                        AppendFromHour( b, parts, instant, upTo, hourInlineSeparator, millisecondInlineSeparator );
                    }
                }
                else
                {
                    b.Append( parts.HasMonth() ? '/' : '-' );
                    Append00( b, instant.Month );
                    if( upTo > TimeSpanUnit.Month )
                    {
                        b.Append( parts.HasDay() ? '/' : '-' );
                        Append00( b, instant.Day );
                        if( upTo > TimeSpanUnit.Day )
                        {
                            AppendFromHour( b, parts, instant, upTo, hourInlineSeparator, millisecondInlineSeparator );
                        }
                    }
                }
            }
        }

        static void AppendFromHour( StringBuilder b,
                                    TimeSpanUnitPathPart parts,
                                    DateTime instant,
                                    TimeSpanUnit upTo,
                                    char hourInlineSeparator,
                                    char millisecondInlineSeparator )
        {
            b.Append( parts.HasHour() ? '/' : hourInlineSeparator );
            Append00( b, instant.Hour );
            if( upTo > TimeSpanUnit.Hour )
            {
                b.Append( parts.HasMinute() ? '/' : '-' );
                Append00( b, instant.Minute );
                if( upTo > TimeSpanUnit.Minute )
                {
                    b.Append( parts.HasSecond() ? '/' : '-' );
                    Append00( b, instant.Second );
                    if( upTo > TimeSpanUnit.Second )
                    {
                        b.Append( parts.HasMillisecond() ? '/' : millisecondInlineSeparator );
                        Append000( b, instant.Millisecond );
                    }
                }
            }
        }

        static void Append000( StringBuilder b, int v )
        {
            if( v < 10 ) b.Append( "00" );
            else if( v < 100 ) b.Append( '0' );
            b.Append( v );
        }

        static void Append00( StringBuilder b, int v )
        {
            if( v < 10 ) b.Append( '0' );
            b.Append( v );
        }
    }

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Semester"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasSemester( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Semester) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.InlineSemester"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasInlineSemester( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.InlineSemester) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Quarter"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasQuarter( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Quarter) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.InlineQuarter"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasInlineQuarter( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.InlineQuarter) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Month"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasMonth( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Month) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Day"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasDay( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Day) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Hour"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasHour( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Hour) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Minute"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasMinute( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Minute) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Second"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasSecond( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Second) != 0;

    /// <summary>
    /// Gets whether <see cref="TimeSpanUnitPathPart.Millisecond"/> bit is set.
    /// </summary>
    /// <param name="parts">This parts.</param>
    /// <returns>Whether the bit is set or not.</returns>
    public static bool HasMillisecond( this TimeSpanUnitPathPart parts ) => (parts & TimeSpanUnitPathPart.Millisecond) != 0;


}
