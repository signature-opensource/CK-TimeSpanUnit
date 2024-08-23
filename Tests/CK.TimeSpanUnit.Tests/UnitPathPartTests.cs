using FluentAssertions;
using NUnit.Framework;
using System;

namespace CK.Core.Tests
{
    public class UnitPathPartTests
    {
        [TestCase( TimeSpanUnit.Year, "2024" )]
        [TestCase( TimeSpanUnit.Semester, "2024-S2" )]
        [TestCase( TimeSpanUnit.Quarter, "2024-Q3" )]
        [TestCase( TimeSpanUnit.Month, "2024-08" )]
        [TestCase( TimeSpanUnit.Day, "2024-08-23" )]
        [TestCase( TimeSpanUnit.Hour, "2024-08-23T16" )]
        [TestCase( TimeSpanUnit.Minute, "2024-08-23T16-42" )]
        [TestCase( TimeSpanUnit.Second, "2024-08-23T16-42-54" )]
        [TestCase( TimeSpanUnit.Millisecond, "2024-08-23T16-42-54.374" )]
        public void None_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 2024, 08, 23, 16, 42, 54, 374 );
            var s = TimeSpanUnitPathPart.None.GetPath( t, upTo );
            s.Should().Be( expected );
        }

        [TestCase( TimeSpanUnit.Year, "2024" )]
        [TestCase( TimeSpanUnit.Semester, "2024/S2" )]
        [TestCase( TimeSpanUnit.Quarter, "2024/S2-Q3" )]
        [TestCase( TimeSpanUnit.Month, "2024/S2-08" )]
        [TestCase( TimeSpanUnit.Day, "2024/S2-08-23" )]
        [TestCase( TimeSpanUnit.Hour, "2024/S2-08-23T16" )]
        [TestCase( TimeSpanUnit.Minute, "2024/S2-08-23T16-42" )]
        [TestCase( TimeSpanUnit.Second, "2024/S2-08-23T16-42-54" )]
        [TestCase( TimeSpanUnit.Millisecond, "2024/S2-08-23T16-42-54.374" )]
        public void Semester_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 2024, 08, 23, 16, 42, 54, 374 );
            var s = TimeSpanUnitPathPart.Semester.GetPath( t, upTo );
            s.Should().Be( expected );
        }

        [TestCase( TimeSpanUnit.Year, "0001" )]
        [TestCase( TimeSpanUnit.Semester, "0001-S1" )]
        [TestCase( TimeSpanUnit.Quarter, "0001/Q1" )]
        [TestCase( TimeSpanUnit.Month, "0001/Q1-02" )]
        [TestCase( TimeSpanUnit.Day, "0001/Q1-02-03" )]
        [TestCase( TimeSpanUnit.Hour, "0001/Q1-02-03T04" )]
        [TestCase( TimeSpanUnit.Minute, "0001/Q1-02-03T04-05" )]
        [TestCase( TimeSpanUnit.Second, "0001/Q1-02-03T04-05-06" )]
        [TestCase( TimeSpanUnit.Millisecond, "0001/Q1-02-03T04-05-06.007" )]
        public void Quarter_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 1, 2, 3, 4, 5, 6, 7 );
            var s = TimeSpanUnitPathPart.Quarter.GetPath( t, upTo );
            s.Should().Be( expected );
        }


        [TestCase( TimeSpanUnit.Year, "0001" )]
        [TestCase( TimeSpanUnit.Semester, "0001/S1" )]
        [TestCase( TimeSpanUnit.Quarter, "0001/S1/Q1" )]
        [TestCase( TimeSpanUnit.Month, "0001/S1/Q1-02" )]
        [TestCase( TimeSpanUnit.Day, "0001/S1/Q1-02-03" )]
        [TestCase( TimeSpanUnit.Hour, "0001/S1/Q1-02-03T04" )]
        [TestCase( TimeSpanUnit.Minute, "0001/S1/Q1-02-03T04-05" )]
        [TestCase( TimeSpanUnit.Second, "0001/S1/Q1-02-03T04-05-06" )]
        [TestCase( TimeSpanUnit.Millisecond, "0001/S1/Q1-02-03T04-05-06.007" )]
        public void SemesterQuarter_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 1, 2, 3, 4, 5, 6, 7 );
            var s = (TimeSpanUnitPathPart.Semester|TimeSpanUnitPathPart.Quarter).GetPath( t, upTo );
            s.Should().Be( expected );
        }

        [TestCase( TimeSpanUnit.Year, "0001" )]
        [TestCase( TimeSpanUnit.Semester, "0001-S1" )]
        [TestCase( TimeSpanUnit.Quarter, "0001-Q1" )]
        [TestCase( TimeSpanUnit.Month, "0001-02" )]
        [TestCase( TimeSpanUnit.Day, "0001/D034" )]
        [TestCase( TimeSpanUnit.Hour, "0001/D034T04" )]
        [TestCase( TimeSpanUnit.Minute, "0001/D034T04-05" )]
        [TestCase( TimeSpanUnit.Second, "0001/D034T04-05-06" )]
        [TestCase( TimeSpanUnit.Millisecond, "0001/D034T04-05-06.007" )]
        public void Day_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 1, 2, 3, 4, 5, 6, 7 );
            var s = (TimeSpanUnitPathPart.Day).GetPath( t, upTo );
            s.Should().Be( expected );
        }

        [TestCase( TimeSpanUnit.Year, "0001" )]
        [TestCase( TimeSpanUnit.Semester, "0001-S1" )]
        [TestCase( TimeSpanUnit.Quarter, "0001/Q1" )]
        [TestCase( TimeSpanUnit.Month, "0001/Q1-02" )]
        [TestCase( TimeSpanUnit.Day, "0001/Q1/D034" )]
        [TestCase( TimeSpanUnit.Hour, "0001/Q1/D034T04" )]
        [TestCase( TimeSpanUnit.Minute, "0001/Q1/D034T04-05" )]
        [TestCase( TimeSpanUnit.Second, "0001/Q1/D034T04-05-06" )]
        [TestCase( TimeSpanUnit.Millisecond, "0001/Q1/D034T04-05-06.007" )]
        public void QuarterDay_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 1, 2, 3, 4, 5, 6, 7 );
            var s = (TimeSpanUnitPathPart.Quarter | TimeSpanUnitPathPart.Day).GetPath( t, upTo );
            s.Should().Be( expected );
        }


        [TestCase( TimeSpanUnit.Year, "0001" )]
        [TestCase( TimeSpanUnit.Semester, "0001-S1" )]
        [TestCase( TimeSpanUnit.Quarter, "0001-Q1" )]
        [TestCase( TimeSpanUnit.Month, "0001-02" )]
        [TestCase( TimeSpanUnit.Day, "0001/D034" )]
        [TestCase( TimeSpanUnit.Hour, "0001/D034/04" )]
        [TestCase( TimeSpanUnit.Minute, "0001/D034/04-05" )]
        [TestCase( TimeSpanUnit.Second, "0001/D034/04-05-06" )]
        [TestCase( TimeSpanUnit.Millisecond, "0001/D034/04-05-06.007" )]
        public void DayHour_GetPath_samples( TimeSpanUnit upTo, string expected )
        {
            DateTime t = new DateTime( 1, 2, 3, 4, 5, 6, 7 );
            var s = (TimeSpanUnitPathPart.Day | TimeSpanUnitPathPart.Hour).GetPath( t, upTo );
            s.Should().Be( expected );
        }

    }
}
