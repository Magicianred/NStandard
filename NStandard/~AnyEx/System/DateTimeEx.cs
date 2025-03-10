﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace NStandard
{
    public static class DateTimeEx
    {
        public static NowScope BeginNowScope() => new();
        public static NowScope BeginNowScope(Func<DateTime, DateTime> store) => new(store);

        public static NowScopeAccessor NowScopes => NowScopeAccessor.Instance;

        /// <summary>
        /// Gets the DateTime(UTC) of UnixMinValue.
        /// </summary>
        /// <returns></returns>
        public static readonly DateTime UnixMinValue = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts the sepecified Unix TimeStamp(seconds) to DateTime(UTC).
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixSeconds(long seconds) => FromUnixMilliseconds(seconds * 1000);

        /// <summary>
        /// Converts the sepecified Unix TimeStamp(milliseconds) to DateTime(UTC).
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixMilliseconds(long milliseconds) => new(milliseconds * 10000 + 621355968000000000, DateTimeKind.Utc);

        /// <summary>
        /// Gets the Unix Timestamp(milliseconds) of the specified DateTime(UTC).
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static long ToUnixTimeMilliseconds(DateTime @this) => (@this.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        /// <summary>
        /// Gets the Unix Timestamp(seconds) of the specified DateTime(UTC).
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static long ToUnixTimeSeconds(DateTime @this) => ToUnixTimeMilliseconds(@this) / 1000;

        /// <summary>
        /// Gets the range of months.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetMonths(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            endDate = new DateTime(endDate.Year, endDate.Month, 1);

            for (var dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
                yield return dt;
        }

        /// <summary>
        /// Gets the range of days.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDays(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                yield return dt;
        }

        private static int PrivateYearDiff(DateTime start, DateTime end)
        {
            if (end < start) throw new ArgumentException("The end time must be after or equal to the start time.");

            var passedYears = end.Year - start.Year;
            var target = start.AddCompleteYears(passedYears);

            if (end < target) return passedYears - 1;
            else return passedYears;
        }

        private static int PrivateMonthDiff(DateTime start, DateTime end)
        {
            if (end < start) throw new ArgumentException("The end time must be after or equal to the start time.");

            var passedYears = end.Year - start.Year;
            var passedMonths = end.Month - start.Month;
            var target = start.AddCompleteMonths(passedYears * 12 + passedMonths);

            if (end < target) return passedYears * 12 + passedMonths - 1;
            else return passedYears * 12 + passedMonths;
        }

        /// <summary>
        /// The number of complete years in the period, similar as DATEDIF(*, *, "Y") function in Excel.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int YearDiff(DateTime start, DateTime end) => MonthDiff(start, end) / 12;

        /// <summary>
        /// The number of complete months in the period, similar as DATEDIF(*, *, "M") function in Excel.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int MonthDiff(DateTime start, DateTime end)
        {
            if (start <= end) return PrivateMonthDiff(start, end);
            else return -PrivateMonthDiff(end, start);
        }

        /// <summary>
        /// The number of complete years in the period, return a double value.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double ExactYearDiff(DateTime start, DateTime end)
        {
            DateTime _start, _end;
            if (start <= end)
            {
                _start = start;
                _end = end;
            }
            else
            {
                _start = end;
                _end = start;
            }

            var diff = PrivateYearDiff(_start, _end);
            var endStart = start.AddCompleteYears(diff);
            var endEnd = endStart.AddCompleteYears(1);
            return diff + (end - endStart).TotalDays / (endEnd - endStart).TotalDays;
        }

        /// <summary>
        /// The number of complete months in the period, return a double value.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double ExactMonthDiff(DateTime start, DateTime end)
        {
            DateTime _start, _end;
            if (start <= end)
            {
                _start = start;
                _end = end;
            }
            else
            {
                _start = end;
                _end = start;
            }

            var diff = PrivateMonthDiff(_start, _end);
            var endStart = start.AddCompleteMonths(diff);
            var endEnd = endStart.AddCompleteMonths(1);
            return diff + (end - endStart).TotalDays / (endEnd - endStart).TotalDays;
        }

        /// <summary>
        /// Gets a DateTime for the specified week of year.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <param name="weekStart"></param>
        /// <returns></returns>
        public static DateTime ParseFromWeek(int year, int week, DayOfWeek weekStart = DayOfWeek.Sunday)
        {
            var day1 = new DateTime(year, 1, 1);
            var week0 = XDateTime.PastDay(day1, weekStart, true);

            if (week0.Year == year)
                week0 = week0.AddDays(-7);

            return week0.AddDays(week * 7);
        }

        /// <summary>
        /// Converts the specified string representation of a date and time to its System.DateTime
        ///     equivalent using the specified format and culture-specific format information.
        ///     The format of the string representation must match the specified format exactly.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ParseExtract(string s, string format)
        {
            return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the specified string representation of a date and time to its System.DateTime
        ///     equivalent using the specified format, culture-specific format information, and
        ///     style. The format of the string representation must match the specified format
        ///     exactly. The method returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseExtract(string s, string format, out DateTime result)
        {
            return DateTime.TryParseExact(s, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
        }

        /// <summary>
        /// Returns the number of days in the specified month and year.
        /// If the specified year is a leap year, return 366, else return 365.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int DaysInYear(int year) => DateTime.IsLeapYear(year) ? 366 : 365;

    }
}
