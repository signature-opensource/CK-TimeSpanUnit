using System;

namespace CK.Core;

/// <summary>
/// Defines how <see cref="DateTime"/> components should be expressed
/// as a path compliant string.
/// See <see cref="TimeSpanUnitPathPartExtensions.GetPath(CK.Core.TimeSpanUnitPathPart, DateTime, CK.Core.TimeSpanUnit)"/>.
/// </summary>
[Flags]
public enum TimeSpanUnitPathPart
{
    /// <summary>
    /// No path part appear. Components are separated by hyphen.
    /// </summary>
    None = 0,

    /// <summary>
    /// Semester (/S1 or /S2) must appear in the path after the year.
    /// </summary>
    Semester = 1,

    /// <summary>
    /// Semester must appear in the path after the year but with an hyphen (-S1 or -S2) instead of a path separator.
    /// </summary>
    InlineSemester = 1 << 1,

    /// <summary>
    /// Quarter (/Q1 to /Q4) must appear in the path, after the year or the semester if <see cref="Semester"/>
    /// or <see cref="InlineSemester"/> are set.
    /// </summary>
    Quarter = 1 << 2,

    /// <summary>
    /// Quarter must appear in the path (after the year or the semester if <see cref="Semester"/>
    /// or <see cref="InlineSemester"/> are set) but with an hyphen (-Q1 to -Q4) instead of a path separator.
    /// </summary>
    InlineQuarter = 1 << 3,

    /// <summary>
    /// The Month (/01 to /12) must appear in the path.
    /// <para>
    /// When not set, the month appear with an hyphen (-01 to -12) except if <see cref="Day"/> is set:
    /// in this case, the day of the year is used (/D001 to /D366).
    /// </para>
    /// </summary>
    Month = 1 << 4,

    /// <summary>
    /// The number of the day must appear in the path (/01 to /31).
    /// If <see cref="Month"/> is not set, the day of the year is used (/D001 to /D366) instead. 
    /// </summary>
    Day = 1 << 5,

    /// <summary>
    /// The hour component must appear in the path (/00 to /24).
    /// When not set an hyphen (-00 to -24) instead of a path separator is used.  
    /// </summary>
    Hour = 1 << 6,

    /// <summary>
    /// The minute component must appear in the path (/00 to /59).
    /// When not set an hyphen (-00 to -59) instead of a path separator is used.  
    /// </summary>
    Minute = 1 << 7,

    /// <summary>
    /// The second component must appear in the path (/00 to /59).
    /// When not set an hyphen (-00 to -59) instead of a path separator is used.  
    /// </summary>
    Second = 1 << 8,

    /// <summary>
    /// The millisecond component must appear in the path (/000 to /999).
    /// When not set an hyphen (-000 to -999) instead of a path separator is used.  
    /// </summary>
    Millisecond = 1 << 9
}
