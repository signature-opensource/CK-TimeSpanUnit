# CK.TimeSpanUnit

This micro package defines 3 basic types (in the CK.Core namespace):

## TimeSpanUnit enumeration
The [TimeSpanUnit](CK.TimeSpanUnit/TimeSpanUnit.cs) defines units of time span:
```csharp
public enum TimeSpanUnit : byte
{
    None,
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
```
Weeks are not supported because it is not that easy (see https://en.wikipedia.org/wiki/ISO_week_date) and
cannot really be grouped by semester or quarter.

If this need to be supported, the ISO definition implemented by [ISOWeek](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.isoweek)
should be used (week starts on monday).
One should also decide how the Day of Week should be rendered: this may be the number (1 to 7),
the `CultureInfo.InvariantCulture`'s `CultureInfo.DateTimeFormat`'s
`DateTimeFormatInfo.DayNames` ("Monday" to "Sunday"), `DateTimeFormatInfo.AbbreviatedDayNames` ("Mon" to "Sun")
or `DateTimeFormatInfo.ShortestDayNames` ("Mo" to "Su").

_Note:_ These names array start with sunday (the en-US way) instead of the ISO monday. This will have to be handled to conform to the ISO rules.

## WeakTimeSpan
The readonly struct [WeakTimeSpan](CK.TimeSpanUnit/WeakTimeSpan.cs) defines a time span as a count of `TimeSpanUnit`. Its string representation (`ToString()`)
is parsable: "Year:3", "Quarter:2", "Minute:5", etc.

This describes a _logical time span_, a time slice. This is NOT isomorph to the .Net [TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan)._

A `WeakTimeSpan` can be normalized when it can be expressed with a smaller count of bigger unit:
- Millisecond => Second => Minute => Hour => Day:
  - "Second:60" => "Minute:1"
  - "Millisecond:18000000" => "Hour:5"
  - "Minute:86400" => "Day:60"
- Quarter => Semester => Year:
  - "Quarter:4" => "Year:1"
  - "Quarter:18" => "Semester:9"
  - "Semester:30" => "Year:15

## TimeSpanUnitPathPart enumeration (and its GetPath)
The [TimeSpanUnitPathPart](CK.TimeSpanUnit/TimeSpanUnitPathPart.cs) enumeration is a bit flag that defines
how `Datetime` components should be as a path compliant string.

The `string GetPath( this TimeSpanUnitPathPart parts, DateTime instant, TimeSpanUnit upTo = TimeSpanUnit.Millisecond)` can build a path based
on the `parts` bit flags that defines the components that must appear as a folder in the path
and the `upTo` parameter that is the precision of the resulting path (this trucates the path after the correspnding component).

A path always starts with the 4-digits year followed by '-' or '/'. When the component is not flagged, hyphens
are used to separate the components except for the time separator that uses the ISO 'T' convention and the milliseconds
that uses the '.' for readability.

With `TimeSpanUnitPathPart.None` (no bit set), the path is "2024-08-23T16-42-54.374" (when `upTo` is `TimeSpanUnit.Millisecond` that is the default). 

For this date, the table below shows the GetPath result for some parts combination:
| Parts | Result | Remarks |
|-------|--------|---------|
|Semester|2024/S2-08-23T16-42-54.374|The semester is a folder.|
|InlineSemester|2024-S2-08-23T16-42-54.374|The semester appears but is not a folder.|
|InlineQuarter,Month,Day|2024-Q3/08/23T16-42-54.374|If there is not many years, this can be intersting...|
|Month,Day,Hour,Minute|2024/08/23/16/42-54.374|To have one folder per hour, Minute path must be set.|
|Day,Hour|2024/D236/16-42-54.374|When Day is specified without Month, the Day of Year is used.|

This path has been designed to non ambiguous and parsable (the parse function has not been implemented but it can be if needed).
