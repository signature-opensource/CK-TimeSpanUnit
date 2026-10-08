# CK.TimeSpanUnit

Utility to properly resolve time and timespan issues.

This repository contains the [CK.TimeSpanUnit](CK.TimeSpanUnit/README.md) micro package that defines
a few basic types (in the `CK.Core` namespace) to work with _logical_ time spans:

- `TimeSpanUnit`: units of time span (Year, Semester, Quarter, Month, Day, Hour, Minute, Second, Millisecond).
- `WeakTimeSpan`: a time span expressed as a count of `TimeSpanUnit` (like "Quarter:2" or "Minute:5") that can
  be normalized and may be "eraligned".
- `DateTimeRange`: a uniquely addressable time range obtained from an eraligned `WeakTimeSpan`.
- `TimeSpanUnitPathPart`: bit flags that render `DateTime` components as path compliant strings.

See the [package documentation](CK.TimeSpanUnit/README.md) for the details.

## Repository layout

| Folder | Content |
|--------|---------|
| [CK.TimeSpanUnit](CK.TimeSpanUnit) | The package itself (targets `net8.0`). |
| [Tests/CK.TimeSpanUnit.Tests](Tests/CK.TimeSpanUnit.Tests) | The unit tests. |

## Building

The solution is [CK-TimeSpanUnit.slnx](CK-TimeSpanUnit.slnx) and requires the .NET SDK specified
in [global.json](global.json):

```
dotnet build
dotnet test
```

## License

This project is licensed under the [GNU Lesser General Public License v3](LICENSE).
