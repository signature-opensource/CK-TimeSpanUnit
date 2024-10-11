using FluentAssertions;
using NUnit.Framework;
using System;
using System.Globalization;

namespace CK.Core.Tests
{

    [TestFixture]
    public class TimeSpanUnitTests
    {

        [TestCase( TimeSpanUnit.Year, "2000/01/01 12:34:56.789", 0, "2000/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Year, "2000/02/10 12:34:56.789", 1, "2001/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Year, "2000/03/20 12:34:56.789", -1, "1999/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Year, "2000/04/30 12:34:56.789", -150, "1850/01/01 00:00:00" )]

        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", 0, "2000/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", 1, "2000/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -1, "1999/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -2, "1999/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -3, "1998/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -4, "1998/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -5, "1997/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", -6, "1997/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", 5, "2002/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", 6, "2003/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Semester, "2000/06/30 12:34:56.789", 7, "2003/07/01 00:00:00" )]

        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 0, "2000/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 1, "2000/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 2, "2000/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 3, "2000/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 4, "2001/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 5, "2001/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 6, "2001/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 7, "2001/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", 8, "2002/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -1, "1999/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -2, "1999/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -3, "1999/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -4, "1999/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -5, "1998/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -6, "1998/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -7, "1998/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -8, "1998/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -9, "1997/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Quarter, "2000/02/15 12:34:56.789", -10, "1997/07/01 00:00:00" )]

        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 0, "2000/02/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 1, "2000/03/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 2, "2000/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 3, "2000/05/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 4, "2000/06/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 5, "2000/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 6, "2000/08/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 7, "2000/09/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 8, "2000/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 9, "2000/11/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 10, "2000/12/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 11, "2001/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 12, "2001/02/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 13, "2001/03/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", 13 + 3 * 12, "2004/03/01 00:00:00" )]

        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -1, "2000/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -2, "1999/12/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -3, "1999/11/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -4, "1999/10/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -5, "1999/09/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -6, "1999/08/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -7, "1999/07/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -8, "1999/06/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -9, "1999/05/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -10, "1999/04/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -11, "1999/03/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -12, "1999/02/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -13, "1999/01/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -14, "1998/12/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -15, "1998/11/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Month, "2000/02/15 12:34:56.789", -16, "1998/10/01 00:00:00" )]

        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 0, "2000/02/15 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 1, "2000/02/16 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 2, "2000/02/17 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 14, "2000/02/29 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 15, "2000/03/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", 365, "2001/02/14 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", -1, "2000/02/14 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", -14, "2000/02/01 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", -15, "2000/01/31 00:00:00" )]
        [TestCase( TimeSpanUnit.Day, "2000/02/15 12:34:56.789", -46, "1999/12/31 00:00:00" )]

        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", 0, "2000/02/15 12:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", 1, "2000/02/15 13:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", 12, "2000/02/16 00:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", 13, "2000/02/16 01:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", -1, "2000/02/15 11:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", -11, "2000/02/15 01:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", -12, "2000/02/15 00:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", -13, "2000/02/14 23:00:00" )]
        [TestCase( TimeSpanUnit.Hour, "2000/02/15 12:34:56.789", -14, "2000/02/14 22:00:00" )]

        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", 0, "2000/02/15 12:34:00" )]
        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", 1, "2000/02/15 12:35:00" )]
        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", 26, "2000/02/15 13:00:00" )]
        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", 60, "2000/02/15 13:34:00" )]
        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", -1, "2000/02/15 12:33:00" )]
        [TestCase( TimeSpanUnit.Minute, "2000/02/15 12:34:56.789", -10*60*24, "2000/02/05 12:34:00" )]

        [TestCase( TimeSpanUnit.Second, "2000/02/15 12:34:56.789", 0, "2000/02/15 12:34:56" )]
        [TestCase( TimeSpanUnit.Second, "2000/02/15 12:34:56.789", 1, "2000/02/15 12:34:57" )]
        [TestCase( TimeSpanUnit.Second, "2000/02/15 12:34:56.789", 60, "2000/02/15 12:35:56" )]
        [TestCase( TimeSpanUnit.Second, "2000/02/15 12:34:56.789", -1, "2000/02/15 12:34:55" )]
        [TestCase( TimeSpanUnit.Second, "2000/02/15 12:34:56.789", -10*60*60*24, "2000/02/05 12:34:56" )]

        [TestCase( TimeSpanUnit.Millisecond, "2000/02/15 12:34:56.789123", 0, "2000/02/15 12:34:56.789" )]
        [TestCase( TimeSpanUnit.Millisecond, "2000/02/15 12:34:56.789123", 1, "2000/02/15 12:34:56.790" )]
        [TestCase( TimeSpanUnit.Millisecond, "2000/02/15 12:34:56.789123", 211, "2000/02/15 12:34:57.000" )]
        [TestCase( TimeSpanUnit.Millisecond, "2000/02/15 12:34:56.789123", -1, "2000/02/15 12:34:56.788" )]
        [TestCase( TimeSpanUnit.Millisecond, "2000/02/15 12:34:56.789123", -10*60*60*24*1000, "2000/02/05 12:34:56.789" )]

        public void GetStart_tests( TimeSpanUnit unit, string date, long offset, string expected )
        {
            DateTime v = DateTime.Parse( date, CultureInfo.InvariantCulture );
            unit.GetStart( v, offset ).Should().Be( DateTime.Parse( expected, CultureInfo.InvariantCulture ) );

            var next = unit.GetStart( v, offset + 1 );
            unit.GetInclusiveEnd( v, offset ).AddTicks( 1 ).Should().Be( next );
        }
    }
}
