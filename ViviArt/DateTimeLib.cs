using System;
using DateTimeExtensions;

namespace ViviArt
{
    public class TimeSet
    {
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
    }

    public static class DateTimeLib
    {
        
        public static DateTime StatDt(this DateTime date, string dateType)
        {
            var _dateType = dateType.ToUpper();
            DateTime d = GeneralDateTimeExtensions.SetTime(date, 0, 0, 0);
            if (_dateType == DateType.Day)
            {
            }
            else if (_dateType == DateType.Week)
            {
                d = GeneralDateTimeExtensions.LastDayOfWeek(d, DayOfWeek.Sunday).AddDays(1);
            }
            else if (_dateType == DateType.Month)
            {
                d = GeneralDateTimeExtensions.FirstDayOfTheMonth(d);
            }
            else if (_dateType == DateType.Year)
            {
                d = new DateTime(d.Year, 1, 1);
            }
            return d;
        }
        public static TimeSet StatDtSet(this DateTime date, string dateType)
        {
            var _dateType = dateType.ToUpper();
            DateTime d = GeneralDateTimeExtensions.SetTime(date, 0, 0, 0);
            DateTime d2 = d;
            switch(dateType)
            {
                case DateType.Day:
                    d2 = d.AddDays(1);
                    break;
                case DateType.Week:
                    d = GeneralDateTimeExtensions.LastDayOfWeek(d, DayOfWeek.Sunday).AddDays(1);
                    d2 = d.AddDays(7);
                    break;
                case DateType.Month:
                    d = GeneralDateTimeExtensions.FirstDayOfTheMonth(d);
                    d2 = GeneralDateTimeExtensions.LastDayOfTheMonth(d).AddDays(1);
                    break;
                case DateType.Year:
                    d = new DateTime(d.Year, 1, 1);
                    d2 = new DateTime(d.Year + 1, 1, 1);
                    break;
            }
            return new TimeSet() { StartDt = d, EndDt = d2 };
        }
        // startDt가 있는 날짜범위는 포함, endDt 날짜범위는 미포함
        public static System.Collections.Generic.IEnumerable<DateTime> StatDtRange(string dateType, DateTime startDt, DateTime endDt)
        {
            var timeSet1 = startDt.StatDtSet(dateType);
            var timeSet2 = endDt.StatDtSet(dateType);

            var tmpDt = timeSet1.StartDt;
            while (true)
            {
                var timeSet = tmpDt.StatDtSet(dateType);
                var statDt = timeSet.StartDt;
                var nextStatDt = timeSet.EndDt;
                if (statDt >= timeSet2.StartDt)
                {
                    break;
                }
                yield return statDt;
                tmpDt = nextStatDt;
            }
        }

        public static string ToString1(this DateTime a)
        {
            return a.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string ToString2(this DateTime a)
        {
            return a.ToString("yyyy-MM-dd");
        }
        public static DateTime ToDateTime1(this string a)
        {
            return DateTime.ParseExact(a, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
        }
        public static DateTime ToDateTime2(this string a)
        {
            return DateTime.ParseExact(a, "yyyy-MM-dd", System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
