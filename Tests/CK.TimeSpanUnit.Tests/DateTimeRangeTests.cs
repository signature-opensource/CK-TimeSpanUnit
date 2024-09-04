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

    }
}
