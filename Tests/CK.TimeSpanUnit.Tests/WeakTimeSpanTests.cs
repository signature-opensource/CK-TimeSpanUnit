using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Globalization;

namespace CK.Core.Tests
{
    public class WeakTimeSpanTests
    {
        [Test]
        public void a_long_is_enough_because_we_work_at_the_Millisecond()
        {
            ulong max = (ulong)TimeSpan.MaxValue.Ticks;
            ulong maxMS = max / TimeSpan.TicksPerMillisecond;
            // 14 bits are available.
            // A TimeSpanUnit is 0 -> 9. we could use only 4 bits but its a enum : byte (and it fits).
            (maxMS & 0b1111_1111_1111_1100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000).Should().Be( 0 );
        }

        [Test]
        public void GetWeakTimeSpan_Sample()
        {
            var a = DateTime.Parse( "2000/01/31 03:04:10", CultureInfo.InvariantCulture );
            var b = DateTime.Parse( "2000/03/31 23:59:59.9999999", CultureInfo.InvariantCulture );
            //TimeSpanUnit.Year.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Year:1" );
            //TimeSpanUnit.Semester.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Semester:1" );
            //TimeSpanUnit.Quarter.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Quarter:1" );
            //TimeSpanUnit.Month.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Month:3" );
            //TimeSpanUnit.Day.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Day:61" );
            //TimeSpanUnit.Hour.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Hour:1461" );
            //TimeSpanUnit.Minute.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Minute:87656" );
            TimeSpanUnit.Second.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Second:5259350" );
            TimeSpanUnit.Millisecond.GetWeakTimeSpan( a, b ).ToString().Should().Be( "Millisecond:5259350000" );
        }

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

        [TestCase( "0001/01/01 00:00:00", "0001/01/01 00:00:00", TimeSpanUnit.Year, "Year:1" )]
        [TestCase( "0001/01/01 00:00:00", "0001/01/01 23:59:59.9999999", TimeSpanUnit.Year, "Year:1" )]
        [TestCase( "0001/01/01 00:00:00", "1969/12/08 14:34:56", TimeSpanUnit.Year, "Year:1969" )]
        [TestCase( "1969/12/08 14:34:56", "0001/01/01 00:00:00", TimeSpanUnit.Year, "Year:1969" )]

        [TestCase( "2000/01/01 14:34:56", "2000/01/01 00:00:00", TimeSpanUnit.Semester, "Semester:1" )]
        [TestCase( "2000/01/01 00:00:00", "2000/06/30 23:59:59.9999999", TimeSpanUnit.Semester, "Semester:1" )]
        [TestCase( "2000/01/01 14:34:56", "2000/07/01 00:00:00", TimeSpanUnit.Semester, "Semester:2" )]
        [TestCase( "2000/01/01 14:34:56", "2000/12/31 23:59:59.9999999", TimeSpanUnit.Semester, "Semester:2" )]
        [TestCase( "2000/01/01 14:34:56", "2001/01/01 00:00:00", TimeSpanUnit.Semester, "Semester:3" )]
        [TestCase( "2000/01/01 14:34:56", "2001/12/31 23:59:59.9999999", TimeSpanUnit.Semester, "Semester:4" )]
        [TestCase( "2000/01/01 14:34:56", "2002/03/01 00:00:00", TimeSpanUnit.Semester, "Semester:5" )]

        [TestCase( "2000/01/01 14:34:56", "2000/01/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:1" )]
        [TestCase( "2000/01/01 00:00:00", "2000/03/31 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:1" )]
        [TestCase( "2000/01/01 14:34:56", "2000/04/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:2" )]
        [TestCase( "2000/01/01 14:34:56", "2000/06/30 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:2" )]
        [TestCase( "2000/01/01 14:34:56", "2000/07/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:3" )]
        [TestCase( "2000/01/01 14:34:56", "2000/09/30 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:3" )]
        [TestCase( "2000/01/01 14:34:56", "2000/10/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:4" )]
        [TestCase( "2000/01/01 14:34:56", "2000/12/31 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:4" )]

        [TestCase( "2000/01/01 14:34:56", "2001/01/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:5" )]
        [TestCase( "2000/01/01 14:34:56", "2001/06/30 00:00:00", TimeSpanUnit.Quarter, "Quarter:6" )]
        [TestCase( "2000/01/01 14:34:56", "2001/07/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:7" )]
        [TestCase( "2000/01/01 14:34:56", "2001/09/30 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:7" )]
        [TestCase( "2000/01/01 14:34:56", "2001/10/01 00:00:00", TimeSpanUnit.Quarter, "Quarter:8" )]
        [TestCase( "2000/01/01 14:34:56", "2001/12/31 23:59:59.9999999", TimeSpanUnit.Quarter, "Quarter:8" )]

        [TestCase( "2000/01/01 14:34:56", "2000/01/01 00:00:00", TimeSpanUnit.Month, "Month:1" )]
        [TestCase( "2000/01/01 14:34:56", "2000/02/01 00:00:00", TimeSpanUnit.Month, "Month:2" )]
        [TestCase( "2000/01/01 14:34:56", "2000/12/01 23:59:59.9999999", TimeSpanUnit.Month, "Month:12" )]
        [TestCase( "2000/01/01 14:34:56", "2001/01/01 00:00:00", TimeSpanUnit.Month, "Month:13" )]

        [TestCase( "2000/01/01 14:34:56", "2000/01/01 00:00:00", TimeSpanUnit.Day, "Day:1" )]
        [TestCase( "2000/01/01 00:00:00", "2000/01/01 23:59:59.9999999", TimeSpanUnit.Day, "Day:1" )]
        [TestCase( "2000/01/01 14:34:56", "2000/01/02 00:00:00", TimeSpanUnit.Day, "Day:2" )]
        [TestCase( "2000/01/01 00:00:00", "2000/01/02 23:59:59.9999999", TimeSpanUnit.Day, "Day:2" )]
        [TestCase( "2000/01/01 14:34:56", "2000/01/03 00:00:00", TimeSpanUnit.Day, "Day:3" )]
        [TestCase( "2000/01/01 14:34:56", "2000/01/08 23:59:59.9999999", TimeSpanUnit.Day, "Day:8" )]

        [TestCase( "2000/01/01 14:10:10", "2000/01/01 14:10:10", TimeSpanUnit.Hour, "Hour:1" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/01 15:10:10", TimeSpanUnit.Hour, "Hour:2" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/01 00:00:00", TimeSpanUnit.Hour, "Hour:15" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/01 23:59:59.9999999", TimeSpanUnit.Hour, "Hour:10" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/02 00:00:00", TimeSpanUnit.Hour, "Hour:11" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/02 01:00:00", TimeSpanUnit.Hour, "Hour:12" )]
        [TestCase( "2000/01/01 14:10:10", "2000/01/05 01:59:59.9999999", TimeSpanUnit.Hour, "Hour:84" )]

        [TestCase( "2000/01/01 14:10:10", "2000/01/01 14:10:10", TimeSpanUnit.Minute, "Minute:1" )]
        [TestCase( "2000/01/01 14:30:00", "2000/01/01 14:30:59.9999999", TimeSpanUnit.Minute, "Minute:1" )]
        [TestCase( "2000/01/01 14:30:00", "2000/01/01 14:31:00", TimeSpanUnit.Minute, "Minute:2" )]
        [TestCase( "2000/01/01 14:30:00", "2000/01/01 15:31:59.9999999", TimeSpanUnit.Minute, "Minute:62" )]

        [TestCase( "2000/01/01 14:10:10", "2000/01/01 14:10:10", TimeSpanUnit.Second, "Second:1" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:59.9999999", TimeSpanUnit.Second, "Second:50" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:31:00", TimeSpanUnit.Second, "Second:51" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 15:31:00.9999999", TimeSpanUnit.Second, "Second:3651" )]

        [TestCase( "2000/01/01 14:10:10", "2000/01/01 14:10:10", TimeSpanUnit.Millisecond, "Millisecond:1" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:10.0009999", TimeSpanUnit.Millisecond, "Millisecond:1" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:10.001", TimeSpanUnit.Millisecond, "Millisecond:2" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:10.010", TimeSpanUnit.Millisecond, "Millisecond:11" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:10.0109999", TimeSpanUnit.Millisecond, "Millisecond:11" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/01 14:30:10.9999999", TimeSpanUnit.Millisecond, "Millisecond:1000" )]
        [TestCase( "2000/01/01 14:30:10", "2000/01/02 14:30:10.9999999", TimeSpanUnit.Millisecond, "Millisecond:86401000" )]

        public void WeakTimeSpan_can_be_computed_between_2_dates( string t1, string t2, TimeSpanUnit unit, string weakTimeSpan )
        {
            DateTime v1 = DateTime.Parse( t1, CultureInfo.InvariantCulture );
            DateTime v2 = DateTime.Parse( t2, CultureInfo.InvariantCulture );
            var s = unit.GetWeakTimeSpan( v1, v2 );
            s.ToString().Should().Be( weakTimeSpan );
            ((s.Count == 1) == unit.SameWeakTimeSpan( v1, v2 )).Should().BeTrue();
        }

        [TestCase( "None:0", false )] // Invalid

        [TestCase( "Semester:1", true )]
        [TestCase( "Semester:2", true )]
        [TestCase( "Semester:3", false )]
        [TestCase( "Semester:4", true )]
        [TestCase( "Semester:5", false )]

        [TestCase( "Quarter:1", true )]
        [TestCase( "Quarter:2", true )]
        [TestCase( "Quarter:3", false )]
        [TestCase( "Quarter:4", true )]
        [TestCase( "Quarter:5", false )]
        [TestCase( "Quarter:6", false )]
        [TestCase( "Quarter:7", false )]
        [TestCase( "Quarter:8", true )]
        [TestCase( "Quarter:9", false )]
        [TestCase( "Quarter:10", false )]
        [TestCase( "Quarter:11", false )]
        [TestCase( "Quarter:12", true )]
        [TestCase( "Quarter:13", false )]

        [TestCase( "Hour:1", true )]
        [TestCase( "Hour:2", true )]
        [TestCase( "Hour:3", true )]
        [TestCase( "Hour:4", true )]
        [TestCase( "Hour:5", false )]
        [TestCase( "Hour:6", true )]
        [TestCase( "Hour:7", false )]
        [TestCase( "Hour:8", true )]
        [TestCase( "Hour:9", false )]
        [TestCase( "Hour:10", false )]
        [TestCase( "Hour:11", false )]
        [TestCase( "Hour:12", true )]
        [TestCase( "Hour:13", false )]
        [TestCase( "Hour:23", false )]
        [TestCase( "Hour:23", false )]
        [TestCase( "Hour:24", true )]
        [TestCase( "Hour:25", false )]
        [TestCase( "Hour:48", true )]
        [TestCase( "Hour:71", false )]
        [TestCase( "Hour:72", true )]
        [TestCase( "Hour:73", false )]

        [TestCase( "Minute:1", true )]
        [TestCase( "Minute:2", true )]
        [TestCase( "Minute:3", true )]
        [TestCase( "Minute:4", true )]
        [TestCase( "Minute:5", true )]
        [TestCase( "Minute:6", true )]
        [TestCase( "Minute:7", false )]
        [TestCase( "Minute:8", false )]
        [TestCase( "Minute:9", false )]
        [TestCase( "Minute:10", true )]
        [TestCase( "Minute:11", false )]
        [TestCase( "Minute:12", true )]
        [TestCase( "Minute:13", false )]
        [TestCase( "Minute:14", false )]
        [TestCase( "Minute:20", true )]
        [TestCase( "Minute:30", true )]
        [TestCase( "Minute:31", false )]
        [TestCase( "Minute:45", false )]
        [TestCase( "Minute:59", false )]
        [TestCase( "Minute:60", true )]     // 1
        [TestCase( "Minute:120", true )]    // 2
        [TestCase( "Minute:180", true )]    // 3
        [TestCase( "Minute:240", true )]    // 4
        [TestCase( "Minute:300", false )]   // 5
        [TestCase( "Minute:360", true )]    // 6
        [TestCase( "Minute:420", false )]   // 7
        [TestCase( "Minute:480", true )]    // 8
        [TestCase( "Minute:540", false )]   // 9
        [TestCase( "Minute:600", false )]   // 10
        [TestCase( "Minute:660", false )]   // 11
        [TestCase( "Minute:720", true )]    // 12
        [TestCase( "Minute:86400", true )]  // 60 jours.

        [TestCase( "Second:60", true )]
        [TestCase( "Second:720", true )]    // 12 minutes    
        [TestCase( "Second:1000", false )]
        [TestCase( "Second:86400", true )]  // 1 day
        [TestCase( "Second:86400", true )]  // 1 day
        [TestCase( "Second:432000", true )] // 5 days

        [TestCase( "Millisecond:1", true )]
        [TestCase( "Millisecond:2", true )]
        [TestCase( "Millisecond:3", false )]
        [TestCase( "Millisecond:4", true )]
        [TestCase( "Millisecond:5", true )]
        [TestCase( "Millisecond:6", false )]
        [TestCase( "Millisecond:7", false )]
        [TestCase( "Millisecond:8", true )]
        [TestCase( "Millisecond:9", false )]
        [TestCase( "Millisecond:10", true )]
        [TestCase( "Millisecond:11", false )]
        [TestCase( "Millisecond:12", false )]
        [TestCase( "Millisecond:25", true )]
        [TestCase( "Millisecond:125", true )]
        [TestCase( "Millisecond:200", true )]
        [TestCase( "Millisecond:250", true )]
        [TestCase( "Millisecond:500", true )]
        [TestCase( "Millisecond:750", false )]
        [TestCase( "Millisecond:10000", true )]
        [TestCase( "Millisecond:86400000", true )] // 1 day

        public void IsEraligned_test( string weakTimeSpan, bool expected )
        {
            WeakTimeSpan.Parse( weakTimeSpan ).IsEraligned.Should().Be( expected );
        }

        [Test]
        public void Days_and_Years_are_always_eraligned()
        {
            var r = Random.Shared.Next( 100 ) + 1;
            new WeakTimeSpan( TimeSpanUnit.Year, r ).IsEraligned.Should().BeTrue();
            new WeakTimeSpan( TimeSpanUnit.Day, r ).IsEraligned.Should().BeTrue();
        }

        [TestCase( "Earliest", "Year:1", "[1000,1001[" )]
        [TestCase( "Centered", "Year:1", "[1000,1001[" )]
        [TestCase( "Latest  ", "Year:1", "[1000,1001[" )]

        [TestCase( "Earliest", "Semester:1", "[1000-S1,1000-S2[" )]
        [TestCase( "Centered", "Semester:1", "[1000-S1,1000-S2[" )]
        [TestCase( "Latest  ", "Semester:1", "[1000-S1,1000-S2[" )]

        [TestCase( "Earliest", "Quarter:1", "[1000-Q1,1000-Q2[" )]
        [TestCase( "Centered", "Quarter:1", "[1000-Q1,1000-Q2[" )]
        [TestCase( "Latest  ", "Quarter:1", "[1000-Q1,1000-Q2[" )]

        [TestCase( "Earliest", "Month:1", "[1000-01,1000-02[" )]
        [TestCase( "Centered", "Month:1", "[1000-01,1000-02[" )]
        [TestCase( "Latest  ", "Month:1", "[1000-01,1000-02[" )]

        [TestCase( "Earliest", "Day:1", "[1000-01-02,1000-01-03[" )]
        [TestCase( "Centered", "Day:1", "[1000-01-02,1000-01-03[" )]
        [TestCase( "Latest  ", "Day:1", "[1000-01-02,1000-01-03[" )]

        [TestCase( "Earliest", "Hour:1", "[1000-01-02T03,1000-01-02T04[" )]
        [TestCase( "Centered", "Hour:1", "[1000-01-02T03,1000-01-02T04[" )]
        [TestCase( "Latest  ", "Hour:1", "[1000-01-02T03,1000-01-02T04[" )]


        [TestCase( "Earliest", "Semester:3", "[0999-S1,1000-S2[" )]
        [TestCase( "Centered", "Semester:3", "[0999-S2,1001-S1[" )]
        [TestCase( "Latest  ", "Semester:3", "[1000-S1,1001-S2[" )]

        [TestCase( "Earliest", "Quarter:3", "[0999-Q3,1000-Q2[" )]
        [TestCase( "Centered", "Quarter:3", "[0999-Q4,1000-Q3[" )]
        [TestCase( "Latest  ", "Quarter:3", "[1000-Q1,1000-Q4[" )]

        //// Hour is 3. 
        [TestCase( "Earliest", "Hour:2", "[1000-01-02T02,1000-01-02T04[" )]
        [TestCase( "Centered", "Hour:2", "[1000-01-02T03,1000-01-02T05[" )] 
        [TestCase( "Latest  ", "Hour:2", "[1000-01-02T03,1000-01-02T05[" )]

        [TestCase( "Earliest", "Hour:3", "[1000-01-02T01,1000-01-02T04[" )]
        [TestCase( "Centered", "Hour:3", "[1000-01-02T02,1000-01-02T05[" )]
        [TestCase( "Latest  ", "Hour:3", "[1000-01-02T03,1000-01-02T06[" )]

        [TestCase( "Earliest", "Hour:4", "[1000-01-02T00,1000-01-02T04[" )]
        [TestCase( "Centered", "Hour:4", "[1000-01-02T02,1000-01-02T06[" )]
        [TestCase( "Latest  ", "Hour:4", "[1000-01-02T03,1000-01-02T07[" )]

        [TestCase( "Earliest", "Hour:7", "[1000-01-01T21,1000-01-02T04[" )]
        [TestCase( "Centered", "Hour:7", "[1000-01-02T00,1000-01-02T07[" )]
        [TestCase( "Latest  ", "Hour:7", "[1000-01-02T03,1000-01-02T10[" )]


        [TestCase( "Earliest", "Minute:13", "[1000-01-02T02-52,1000-01-02T03-05[" )]
        [TestCase( "Centered", "Minute:13", "[1000-01-02T02-58,1000-01-02T03-11[" )]
        [TestCase( "Latest  ", "Minute:13", "[1000-01-02T03-04,1000-01-02T03-17[" )]

        public void Get_Earliest_Centered_Latest_range_can_be_called_on_any_WeakTimeSpan( string mode, string weakTimeSpan, string expected )
        {
            var kind = Random.Shared.Next( 3 ) switch { 0 => DateTimeKind.Local, 1 => DateTimeKind.Local, _ => DateTimeKind.Unspecified };
            var t = new DateTime( 1000, 1, 2, 3, 4, 5, 6, 7, kind );

            var span = WeakTimeSpan.Parse( weakTimeSpan );

            var (start, end) = mode == "Earliest"
                                ? span.GetEarliestRange( t )
                                : mode == "Centered"
                                    ? span.GetCenteredRange( t )
                                    : span.GetLatestRange( t );
            start.Kind.Should().Be( end.Kind ).And.Be( kind );
            // The end is excluded but GetWeakTimeSpan computes the span that fully covers
            // the range.
            span.Unit.GetWeakTimeSpan( start, end.AddTicks( -1 ) ).Should().Be( span );

            t.Should().BeOnOrAfter( start ).And.BeBefore( end );

            DateTimeRange.ToString( span.Unit, start, end ).Should().Be( expected );
        }

    }
}
