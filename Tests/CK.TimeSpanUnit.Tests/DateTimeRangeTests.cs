using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;

namespace CK.Core.Tests
{
    [TestFixture]
    public class DateTimeRangeTests
    {
        [TestCase( "Year:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Year:1", 1, "0002/01/01 00:00:00" )]
        [TestCase( "Year:1", 2, "0003/01/01 00:00:00" )]
        [TestCase( "Year:2", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Year:2", 1, "0003/01/01 00:00:00" )]
        [TestCase( "Year:2", 2, "0005/01/01 00:00:00" )]
        [TestCase( "Year:10", 200, "2001/01/01 00:00:00" )]

        [TestCase( "Semester:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Semester:1", 1, "0001/07/01 00:00:00" )]
        [TestCase( "Semester:1", 2, "0002/01/01 00:00:00" )]
        [TestCase( "Semester:1", 3, "0002/07/01 00:00:00" )]
        [TestCase( "Semester:2", 2, "0003/01/01 00:00:00" )]

        [TestCase( "Quarter:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Quarter:1", 1, "0001/04/01 00:00:00" )]
        [TestCase( "Quarter:1", 2, "0001/07/01 00:00:00" )]
        [TestCase( "Quarter:1", 3, "0001/10/01 00:00:00" )]
        [TestCase( "Quarter:1", 4, "0002/01/01 00:00:00" )]
        [TestCase( "Quarter:1", 5, "0002/04/01 00:00:00" )]
        [TestCase( "Quarter:1", 6, "0002/07/01 00:00:00" )]
        [TestCase( "Quarter:1", 7, "0002/10/01 00:00:00" )]

        [TestCase( "Month:1", 0,  "0001/01/01 00:00:00" )]
        [TestCase( "Month:1", 1,  "0001/02/01 00:00:00" )]
        [TestCase( "Month:1", 7,  "0001/08/01 00:00:00" )]
        [TestCase( "Month:1", 11, "0001/12/01 00:00:00" )]
        [TestCase( "Month:1", 12, "0002/01/01 00:00:00" )]
        [TestCase( "Month:1", 13, "0002/02/01 00:00:00" )]
        [TestCase( "Month:1", 14, "0002/03/01 00:00:00" )]
        [TestCase( "Month:1", 14, "0002/03/01 00:00:00" )]
        [TestCase( "Month:4", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Month:4", 1, "0001/05/01 00:00:00" )]
        [TestCase( "Month:4", 2, "0001/09/01 00:00:00" )]
        [TestCase( "Month:4", 3, "0002/01/01 00:00:00" )]

        [TestCase( "Day:1", 0,  "0001/01/01 00:00:00" )]
        [TestCase( "Day:1", 1, "0001/01/02 00:00:00" )]
        [TestCase( "Day:1", 30, "0001/01/31 00:00:00" )]
        [TestCase( "Day:1", 50, "0001/02/20 00:00:00" )]
        [TestCase( "Day:50", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Day:50", 1, "0001/02/20 00:00:00" )]
        [TestCase( "Day:50", 2, "0001/04/11 00:00:00" )]
        [TestCase( "Day:50", 3, "0001/05/31 00:00:00" )]
        [TestCase( "Day:50", 4, "0001/07/20 00:00:00" )]

        [TestCase( "Hour:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Hour:1", 1, "0001/01/01 01:00:00" )]
        [TestCase( "Hour:1", 2, "0001/01/01 02:00:00" )]
        [TestCase( "Hour:1", 3, "0001/01/01 03:00:00" )]

        [TestCase( "Hour:2", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Hour:2", 1, "0001/01/01 02:00:00" )]
        [TestCase( "Hour:2", 2, "0001/01/01 04:00:00" )]
        [TestCase( "Hour:2", 3, "0001/01/01 06:00:00" )]
        [TestCase( "Hour:2", 6, "0001/01/01 12:00:00" )]
        [TestCase( "Hour:2", 12, "0001/01/02 00:00:00" )]
        [TestCase( "Hour:2", 13, "0001/01/02 02:00:00" )]

        [TestCase( "Hour:8", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Hour:8", 1, "0001/01/01 08:00:00" )]
        [TestCase( "Hour:8", 2, "0001/01/01 16:00:00" )]
        [TestCase( "Hour:8", 3, "0001/01/02 00:00:00" )]
        [TestCase( "Hour:8", 4, "0001/01/02 08:00:00" )]
        [TestCase( "Hour:8", 5, "0001/01/02 16:00:00" )]

        [TestCase( "Hour:12", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Hour:12", 1, "0001/01/01 12:00:00" )]
        [TestCase( "Hour:12", 2, "0001/01/02 00:00:00" )]
        [TestCase( "Hour:12", 3, "0001/01/02 12:00:00" )]
        [TestCase( "Hour:48", 4, "0001/01/09 00:00:00" )]

        [TestCase( "Minute:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Minute:1", 1, "0001/01/01 00:01:00" )]
        [TestCase( "Minute:1", 2, "0001/01/01 00:02:00" )]
        [TestCase( "Minute:30", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Minute:30", 1, "0001/01/01 00:30:00" )]
        [TestCase( "Minute:30", 2, "0001/01/01 01:00:00" )]
        [TestCase( "Minute:30", 3, "0001/01/01 01:30:00" )]
        [TestCase( "Minute:30", 4, "0001/01/01 02:00:00" )]
        [TestCase( "Minute:60", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Minute:60", 1, "0001/01/01 01:00:00" )]
        [TestCase( "Minute:60", 2, "0001/01/01 02:00:00" )]
        [TestCase( "Minute:1440", 0, "0001/01/01 00:00:00" )] // 24*60: one day
        [TestCase( "Minute:1440", 1, "0001/01/02 00:00:00" )]
        [TestCase( "Minute:1440", 20, "0001/01/21 00:00:00" )]
        [TestCase( "Minute:14400", 0, "0001/01/01 00:00:00" )] // 10 days
        [TestCase( "Minute:14400", 1, "0001/01/11 00:00:00" )]
        [TestCase( "Minute:14400", 2, "0001/01/21 00:00:00" )]

        [TestCase( "Second:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Second:1", 1, "0001/01/01 00:00:01" )]
        [TestCase( "Second:1", 2, "0001/01/01 00:00:02" )]
        [TestCase( "Second:10", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Second:10", 1, "0001/01/01 00:00:10" )]
        [TestCase( "Second:10", 2, "0001/01/01 00:00:20" )]
        [TestCase( "Second:10", 3, "0001/01/01 00:00:30" )]
        [TestCase( "Second:10", 4, "0001/01/01 00:00:40" )]
        [TestCase( "Second:60", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Second:60", 1, "0001/01/01 00:01:00" )]
        [TestCase( "Second:60", 2, "0001/01/01 00:02:00" )]
        [TestCase( "Second:3600", 0, "0001/01/01 00:00:00" )] // 60*60: one hour
        [TestCase( "Second:3600", 1, "0001/01/01 01:00:00" )]
        [TestCase( "Second:3600", 2, "0001/01/01 02:00:00" )]
        [TestCase( "Second:86400", 0, "0001/01/01 00:00:00" )] // One day
        [TestCase( "Second:86400", 1, "0001/01/02 00:00:00" )]
        [TestCase( "Second:86400", 2, "0001/01/03 00:00:00" )]
        [TestCase( "Second:8640000", 1, "0001/04/11 00:00:00" )] // 100 days


        [TestCase( "Millisecond:1", 0, "0001/01/01 00:00:00" )]
        [TestCase( "Millisecond:1", 100, "0001/01/01 00:00:00.100" )]
        [TestCase( "Millisecond:2", 20, "0001/01/01 00:00:00.040" )]
        [TestCase( "Millisecond:4", 200, "0001/01/01 00:00:00.800" )]
        [TestCase( "Millisecond:4", 500, "0001/01/01 00:00:02" )]
        [TestCase( "Millisecond:10000", 1, "0001/01/01 00:00:10" )]
        [TestCase( "Millisecond:86400000", 1, "0001/01/02 00:00:00" )] // One day
        [TestCase( "Millisecond:86400000000", 1, "0003/09/28 00:00:00" )] // 1000 days

        public void DateTimeRange_by_Index( string weakTimeSpan, long index, string expectedStartDate )
        {
            var s = WeakTimeSpan.Parse( weakTimeSpan );
            var r = s.GetDateTimeRange( index );
            r.Start.Should().Be( DateTime.Parse( expectedStartDate, CultureInfo.InvariantCulture ) );
            r.Index.Should().Be( index );
        }

        [TestCase( "Year:1", "0001/01/01 00:00:00", "[0001,0002[" )]
        [TestCase( "Year:1", "2024/09/09 13:24:41", "[2024,2025[" )]
        [TestCase( "Year:10", "2024/09/09 13:24:41", "[2021,2031[" )]
        [TestCase( "Year:100", "2024/09/09 13:24:41", "[2001,2101[" )]
        [TestCase( "Year:512", "2024/09/09 13:24:41", "[1537,2049[" )]
        [TestCase( "Year:1024", "2024/09/09 13:24:41", "[1025,2049[" )]
        // Other semester count than 1 must be even and are normalized to years.
        [TestCase( "Semester:1", "2024/09/09 13:24:41", "[2024-S2,2025-S1[" )]
        [TestCase( "Semester:2", "2024/09/09 13:24:41", "[2024,2025[" )]
        // Other quarter count than 1 must be even and are normalized to semesters or years.
        [TestCase( "Quarter:1", "2024/09/09 13:24:41", "[2024-Q3,2024-Q4[" )]
        [TestCase( "Quarter:2", "2024/09/09 13:24:41", "[2024-S2,2025-S1[" )]
        [TestCase( "Quarter:4", "2024/09/09 13:24:41", "[2024,2025[" )]

        [TestCase( "Month:1", "2024/09/09 13:24:41", "[2024-09,2024-10[" )]
        [TestCase( "Month:2", "2024/09/09 13:24:41", "[2024-09,2024-11[" )]
        [TestCase( "Month:3", "2024/09/09 13:24:41", "[2024-Q3,2024-Q4[" )] // Normalized to Quarter.
        [TestCase( "Month:4", "2024/09/09 13:24:41", "[2024-09,2025-01[" )]
        [TestCase( "Month:6", "2024/09/09 13:24:41", "[2024-S2,2025-S1[" )] // Normalized to Semester.
        [TestCase( "Month:24", "2024/09/09 13:24:41", "[2023,2025[" )] // Normalized to Year:2.

        [TestCase( "Day:1", "2024/09/09 13:24:41", "[2024-09-09,2024-09-10[" )]
        [TestCase( "Day:10", "2024/09/09 13:24:41", "[2024-09-02,2024-09-12[" )]
        [TestCase( "Day:20", "2024/09/09 13:24:41", "[2024-08-23,2024-09-12[" )]
        [TestCase( "Day:30", "2024/09/09 13:24:41", "[2024-08-13,2024-09-12[" )]
        [TestCase( "Day:40", "2024/09/09 13:24:41", "[2024-08-23,2024-10-02[" )]

        [TestCase( "Day:11", "2024/09/09 13:24:41", "[2024-09-06,2024-09-17[" )]
        [TestCase( "Day:12", "2024/09/09 13:24:41", "[2024-08-31,2024-09-12[" )]

        [TestCase( "Hour:1", "2024/09/09 13:24:41", "[2024-09-09T13,2024-09-09T14[" )]
        [TestCase( "Hour:2", "2024/09/09 13:24:41", "[2024-09-09T12,2024-09-09T14[" )]
        [TestCase( "Hour:3", "2024/09/09 13:24:41", "[2024-09-09T12,2024-09-09T15[" )]
        [TestCase( "Hour:4", "2024/09/09 13:24:41", "[2024-09-09T12,2024-09-09T16[" )]
        [TestCase( "Hour:6", "2024/09/09 13:24:41", "[2024-09-09T12,2024-09-09T18[" )]
        [TestCase( "Hour:8", "2024/09/09 13:24:41", "[2024-09-09T08,2024-09-09T16[" )]
        [TestCase( "Hour:12", "2024/09/09 13:24:41", "[2024-09-09T12,2024-09-10T00[" )]
        [TestCase( "Hour:48", "2024/09/09 13:24:41", "[2024-09-08,2024-09-10[" )]

        [TestCase( "Minute:1", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-25[" )]
        [TestCase( "Minute:2", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-26[" )]
        [TestCase( "Minute:3", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-27[" )]
        [TestCase( "Minute:4", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-28[" )]
        [TestCase( "Minute:5", "2024/09/09 13:24:41", "[2024-09-09T13-20,2024-09-09T13-25[" )]
        [TestCase( "Minute:6", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-30[" )]
        [TestCase( "Minute:12", "2024/09/09 13:24:41", "[2024-09-09T13-24,2024-09-09T13-36[" )]
        [TestCase( "Minute:1440", "2024/09/09 13:24:41", "[2024-09-09,2024-09-10[" )]

        [TestCase( "Second:1", "2024/09/09 13:24:41", "[2024-09-09T13-24-41,2024-09-09T13-24-42[" )]
        [TestCase( "Second:2", "2024/09/09 13:24:41", "[2024-09-09T13-24-40,2024-09-09T13-24-42[" )]
        [TestCase( "Second:3", "2024/09/09 13:24:41", "[2024-09-09T13-24-39,2024-09-09T13-24-42[" )]
        [TestCase( "Second:4", "2024/09/09 13:24:41", "[2024-09-09T13-24-40,2024-09-09T13-24-44[" )]
        [TestCase( "Second:5", "2024/09/09 13:24:41", "[2024-09-09T13-24-40,2024-09-09T13-24-45[" )]
        [TestCase( "Second:6", "2024/09/09 13:24:41", "[2024-09-09T13-24-36,2024-09-09T13-24-42[" )]
        [TestCase( "Second:12", "2024/09/09 13:24:41", "[2024-09-09T13-24-36,2024-09-09T13-24-48[" )]
        [TestCase( "Second:3600", "2024/09/09 13:24:41", "[2024-09-09T13,2024-09-09T14[" )]

        [TestCase( "Millisecond:1", "2024/09/09 13:24:41", "[2024-09-09T13-24-41.000,2024-09-09T13-24-41.001[" )]
        [TestCase( "Millisecond:1", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.123,2024-09-09T13-24-41.124[" )]
        [TestCase( "Millisecond:2", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.122,2024-09-09T13-24-41.124[" )]
        [TestCase( "Millisecond:4", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.120,2024-09-09T13-24-41.124[" )]
        [TestCase( "Millisecond:8", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.120,2024-09-09T13-24-41.128[" )]
        [TestCase( "Millisecond:50", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.100,2024-09-09T13-24-41.150[" )]
        [TestCase( "Millisecond:100", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.100,2024-09-09T13-24-41.200[" )]
        [TestCase( "Millisecond:125", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.000,2024-09-09T13-24-41.125[" )]
        [TestCase( "Millisecond:250", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-41.000,2024-09-09T13-24-41.250[" )]
        [TestCase( "Millisecond:10000", "2024/09/09 13:24:41.12345", "[2024-09-09T13-24-40,2024-09-09T13-24-50[" )]

        public void DateTimeRange_by_DateTime( string weakTimeSpan, string dateTime, string expectedRange )
        {
            var s = WeakTimeSpan.Parse( weakTimeSpan );
            var r = s.GetDateTimeRange( DateTime.Parse( dateTime, CultureInfo.InvariantCulture ) );
            r.ToString().Should().Be( expectedRange );
        }

    }
}
