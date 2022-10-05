// --------------------------------------------------------------------------
// <copyright file="DateTimeExtension.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Extensions
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Date Time Extension.
    /// </summary>
    public static class DateTimeExtension
    {
        public const string ShortDateFormat = "MMM dd, yyyy";

        public const string MonthFormat = "MMM";

        public const string ShortDateFormatWithSlash = "MM/dd/yyyy";

        public const string LongDateTimeFormat = "MMM dd, yyyy hh:mm tt";

        public const string MonthYearFormat = "MMM yyyy";

        public const string HourMinuteFormat = "HH:mm";

        public const string HourMinuteMiddayTypeFormat = "hh:mm tt";

        private const int NumberDayInWeek = 7;

        private static readonly DateTime MinDate = new DateTime(1900, 1, 1);

        private static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 999);

        [DebuggerStepThrough]
        public static bool IsValid(this DateTime target)
        {
            return (target >= MinDate) && (target <= MaxDate);
        }

        [DebuggerStepThrough]
        public static bool IsInRange(this DateTime target, DateTime fromDate, DateTime toDate)
        {
            return (target >= fromDate) && (target <= toDate);
        }

        public static string ToShortFormatedDate(this DateTime date)
        {
            return date.ToString(ShortDateFormat);
        }

        public static string ToHourMinuteFormated(this DateTime date)
        {
            return date.ToString(HourMinuteFormat);
        }

        public static string ToHourMinuteMiddayTypeFormated(this DateTime date)
        {
            return date.ToString(HourMinuteMiddayTypeFormat);
        }

        public static string ToShortFormatedDateWithSlash(this DateTime date)
        {
            return date.ToString(ShortDateFormatWithSlash);
        }

        public static string ToLongFormatedDateTime(this DateTime date)
        {
            return date.ToString(LongDateTimeFormat);
        }

        public static string ToMonth(this DateTime date)
        {
            return date.ToString(MonthFormat);
        }

        public static DateTime FirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dateTime)
        {
            return dateTime.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        public static DateTime LastDayOfYear(this DateTime dateTime)
        {
            return dateTime.FirstDayOfYear().AddYears(1).AddDays(-1);
        }

        public static DateTime GetEndOfDate(this DateTime date)
            => new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddSeconds(-1);

        public static DateTime GetStartOfDate(this DateTime date)
            => new DateTime(date.Year, date.Month, date.Day);

        public static string ToMonthYear(this DateTime date)
        {
            return date.ToString(MonthYearFormat);
        }

        public static DateTime FirstDayOfWeek(this DateTime dateTime)
        {
            var dayOfWeek = (int)dateTime.DayOfWeek;
            return dateTime.AddDays(-dayOfWeek);
        }

        public static DateTime LastDayOfWeek(this DateTime dateTime)
        {
            return dateTime.FirstDayOfWeek().AddDays(NumberDayInWeek - 1);
        }
    }
}