namespace CK.Core;

/// <summary>
/// Defines the units of time span.
/// </summary>
public enum TimeSpanUnit : byte
{
    /// <summary>
    /// Non applicable.
    /// </summary>
    None = 0,
    Year,
    Semester,
    Quarter,
    Month,
    Day,
    Hour,
    Minute,
    Second,
    Millisecond
}
