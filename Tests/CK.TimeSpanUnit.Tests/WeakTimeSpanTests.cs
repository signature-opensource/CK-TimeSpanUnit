using FluentAssertions;
using NUnit.Framework;

namespace CK.Core.Tests
{
    public class WeakTimeSpanTests
    {
        [TestCase( TimeSpanUnit.Year, 1000 )]
        [TestCase( TimeSpanUnit.Semester, 1 )]
        [TestCase( TimeSpanUnit.Quarter, 4 )]
        [TestCase( TimeSpanUnit.Month, 15 )]
        [TestCase( TimeSpanUnit.Day, 100 )]
        [TestCase( TimeSpanUnit.Hour, 2 )]
        [TestCase( TimeSpanUnit.Minute, 30 )]
        [TestCase( TimeSpanUnit.Second, 3600 )]
        [TestCase( TimeSpanUnit.Millisecond, 35 )]
        public void ToString_and_Parse( TimeSpanUnit unit, int count )
        {
            var ts = new WeakTimeSpan( unit, count );
            ts.IsValid.Should().BeTrue();
            var s = ts.ToString();
            var tsBack = WeakTimeSpan.Parse( s );
            tsBack.Should().Be( ts );
        }

        [TestCase( "none" )]
        [TestCase( "NONE:" )]
        [TestCase( "NONE : 45" )]
        public void None_can_be_parsed( string s )
        {
            var none = WeakTimeSpan.Parse( s );
            none.Count.Should().Be( 0 );
            none.Unit.Should().Be( TimeSpanUnit.None );
            none.IsValid.Should().BeFalse();
        }

        [TestCase( "Millisecond:1000", "Second:1" )]
        [TestCase( "Millisecond:60000", "Minute:1" )]
        [TestCase( "Millisecond:18000000", "Hour:5" )]
        [TestCase( "Millisecond:172800000", "Day:2" )]

        [TestCase( "Second:60", "Minute:1" )]
        [TestCase( "Second:3600", "Hour:1" )]
        [TestCase( "Second:86400", "Day:1" )]

        [TestCase( "Minute:60", "Hour:1" )]
        [TestCase( "Minute:3600", "Hour:60" )]
        [TestCase( "Minute:86400", "Day:60" )]

        [TestCase( "Hour:24", "Day:1" )]
        [TestCase( "Hour:480", "Day:20" )]

        [TestCase( "Month:3", "Quarter:1" )]
        [TestCase( "Month:6", "Semester:1" )]
        [TestCase( "Month:12", "Year:1" )]
        [TestCase( "Month:9", "Quarter:3" )]
        [TestCase( "Month:18", "Semester:3" )]
        [TestCase( "Month:36", "Year:3" )]

        [TestCase( "Quarter:4", "Year:1" )]
        [TestCase( "Quarter:2", "Semester:1" )]
        [TestCase( "Quarter:40", "Year:10" )]
        [TestCase( "Quarter:18", "Semester:9" )]

        [TestCase( "Semester:2", "Year:1" )]
        [TestCase( "Semester:30", "Year:15" )]
        public void Normalize_in_action( string s, string expected )
        {
            WeakTimeSpan.Parse( s ).Normalize().ToString().Should().Be( expected );
        }

        [TestCase( "Month:1" )]
        [TestCase( "Month:2" )] // 3
        [TestCase( "Month:4" )]
        [TestCase( "Month:5" )] // 6
        [TestCase( "Month:7" )]
        [TestCase( "Month:8" )] // 9
        [TestCase( "Month:10" )]
        [TestCase( "Month:11" )] // 12
        [TestCase( "Month:13" )]
        [TestCase( "Month:14" )] // 15
        [TestCase( "Month:16" )]
        [TestCase( "Month:17" )] // 18
        [TestCase( "Month:19" )]
        [TestCase( "Month:20" )] // 21
        [TestCase( "Month:22" )]
        [TestCase( "Month:23" )] // 24
        [TestCase( "Month:25" )]

        [TestCase( "Quarter:1" )]
        [TestCase( "Quarter:3" )]
        [TestCase( "Quarter:5" )]
        [TestCase( "Quarter:7" )]
        [TestCase( "Quarter:9" )]

        [TestCase( "Semester:1" )]
        [TestCase( "Semester:3" )]
        [TestCase( "Semester:5" )]
        [TestCase( "Semester:7" )]

        [TestCase( "Day:1" )]
        [TestCase( "Day:29" )]
        [TestCase( "Day:30" )]
        [TestCase( "Day:31" )]

        [TestCase( "Millisecond:999" )]
        [TestCase( "Millisecond:59999" )]
        [TestCase( "Millisecond:18000001" )]
        [TestCase( "Millisecond:172800001" )]

        public void Not_Normalized( string s )
        {
            WeakTimeSpan.Parse( s ).Normalize().ToString().Should().Be( s );
        }
    }
}
