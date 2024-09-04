# CK.TimeSpanUnit

This micro package defines 4 basic types (in the CK.Core namespace):

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
Only 2 operations are supported on this unit (through extension methods):
- `unit.GetStart( DateTime t, long offset = 0 )` returns the first DateTime on the unit that contains `t`.
- `unit.GetEnd( DateTime t, long offset = 0 )` returns the inclusive end of the unit: it is the start of the next span (the `GetStart( t, offset+1)`)
minus one Tick (100 nanoseconds).

### Weeks are not supported
Because it is not that easy (see https://en.wikipedia.org/wiki/ISO_week_date) and
cannot really be grouped by semester or quarter and even year.

If this need to be supported, the ISO definition implemented by [ISOWeek](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.isoweek)
should be used (week starts on monday).
One should also decide how the Day of Week should be rendered: this may be the number (1 to 7),
the `CultureInfo.InvariantCulture`'s `CultureInfo.DateTimeFormat`'s
`DateTimeFormatInfo.DayNames` ("Monday" to "Sunday"), `DateTimeFormatInfo.AbbreviatedDayNames` ("Mon" to "Sun")
or `DateTimeFormatInfo.ShortestDayNames` ("Mo" to "Su").

_Note:_ These names array start with sunday (the en-US way) instead of the ISO monday. This will have to be handled to conform to the ISO rules.

## WeakTimeSpan, alignment and DateTimeRange
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
  - "Semester:30" => "Year:15"

This type is not a "mathematical" type, it doesn't support any operation. The `WeakTimeSpan.Count` is necessarily positive (cannot be 0 or negative).
A `WeakTimeSpan` can be computed from 2 `DateTime`:
```csharp
var a = DateTime.Parse( "2000/01/31 03:04:10", CultureInfo.InvariantCulture );
var b = DateTime.Parse( "2000/03/31 23:59:59.9999999", CultureInfo.InvariantCulture );
TimeSpanUnit.Year.GetWeakTimeSpan( a, b )        // => "Year:1"
TimeSpanUnit.Semester.GetWeakTimeSpan( a, b )    // => "Semester:1"
TimeSpanUnit.Quarter.GetWeakTimeSpan( a, b )     // => "Quarter:1"
TimeSpanUnit.Month.GetWeakTimeSpan( a, b )       // => "Month:3"
TimeSpanUnit.Day.GetWeakTimeSpan( a, b )         // => "Day:61"
TimeSpanUnit.Hour.GetWeakTimeSpan( a, b )        // => "Hour:1461"
TimeSpanUnit.Minute.GetWeakTimeSpan( a, b )      // => "Minute:87656"
TimeSpanUnit.Second.GetWeakTimeSpan( a, b )      // => "Second:5259350"
TimeSpanUnit.Millisecond.GetWeakTimeSpan( a, b ) // => "Millisecond:5259350000"
```
When the `Count` is 1, it means that the 2 dates are in the same unit of time. The `bool SameWeakTimeSpan( DateTime t1, DateTime t2 )`
extension method can test that without creating a `WeakTimeSpan`.

_Note:_ Because we work at most in milliseconds, the `WeakTimeSpan.Count` can be encoded in no more than 50 bits. A WeakTimeSpan 
fits in a 64 bits number.
 
A `WeakTimeSpan` is "aligned" if it fits into its parent unit and integrally divides it (no remainder).
Based on the **normalized form** of a `WeakTimeSpan`:

- When `WeakTimeSpan.Count` is 1 then it is always aligned.
- Else when `WeakTimeSpan.Unit` is:
  - `Year` then it is always aligned.
  - `Semester` is aligned if its `Count` is a multiple of 2 (i.e. it can be normalized to one or more years).
  - `Quarter` is if its `Count` is 2 or a multiple of 4 (i.e. it can be normalized to one semester or one or more years).
  - `Month` is aligned if its `Count` is 2, 3, 4 or 6 or is a multiple of 12 (i.e. it can be normalized to one or more years).
  - `Days` then it is always aligned.
  - `Hour` is aligned if its `Count` is 2, 3, 4, 6, 8, 12 (divisors of 24) or is a multiple of 24 (i.e. it can be normalized to one or more days). 
  - `Minute` is aligned if its `Count` is 2, 3, 4, 5, 6, 10, 12, 15, 20 or 30 (divisors of 60) or it must be an integral
    count of hours that must be aligned (that is the normalized form in hour must exist and be aligned).
  - `Second` is aligned if its `Count` is 2, 3, 4, 5, 6, 10, 12, 15, 20 or 30 (divisors of 60) or it must be an integral
    count of minutes that must be aligned (that is the normalized form in minutes must exist and be aligned). 
  - `Millisecond` is aligned if its `Count` is 2, 4, 5, 8, 10, 20, 25, 40, 50, 100, 125, 200, 250 or 500 (divisors of 1000) or it must be an integral
    count of seconds that must be aligned (that is the normalized form in seconds must exist and be aligned). 


An aligned `WeakTimeSpan` divides the time in computable "time ranges" (say "range" here to avoid reusing "span"). Any `DateTime`
can be associated to a unique "time range" that contains it. A [`DateTimeRange`](CK.TimeSpanUnit/DateTimeRange.cs) has a `DateTime Start`,
`DateTime End`, its `WeakTimeSpan Span` and a 0 based `long Index` that identifies it its `Span`.



## TimeSpanUnitPathPart enumeration (and its GetPath)
The [TimeSpanUnitPathPart](CK.TimeSpanUnit/TimeSpanUnitPathPart.cs) enumeration is a bit flag that defines
how `Datetime` components should be as a path compliant string.

The `string GetPath( this TimeSpanUnitPathPart parts, DateTime instant, TimeSpanUnit upTo = TimeSpanUnit.Millisecond)` can build a path based
on the `parts` bit flags that defines the components that must appear as a folder in the path
and the `upTo` parameter that is the precision of the resulting path (this truncates the path after the correspnding component).

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
