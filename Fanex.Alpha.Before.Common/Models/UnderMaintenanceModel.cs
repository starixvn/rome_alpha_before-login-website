namespace Fanex.Alpha.Before.Common.Models
{
    using System;

    public class UnderMaintenanceModel
    {
        private const string TaipeiStandardTime = "Taipei Standard Time";

        public UnderMaintenanceModel(
            bool isUm,
            string fromTimeText,
            string toTimeText,
            bool hasError)
        {
            IsUM = isUm;
            FromTimeText = fromTimeText;
            ToTimeText = toTimeText;
            HasError = hasError;
        }

        public bool IsUM { get; }

        public string FromTimeText { get; }

        public string ToTimeText { get; }

        public DateTime EndTime { get; private set; }

        public TimeSpan CountdownTime { get; private set; }

        public bool HasError { get; }

        public bool ShowCountDown()
        {
            if (string.IsNullOrWhiteSpace(ToTimeText)
                || ToTimeText.Length < 5
                || (ToTimeText.Length > 2 && ToTimeText[2] != ':'))
            {
                return false;
            }

            int endHours;
            int endMinutes;

            if (int.TryParse(ToTimeText.Substring(0, 2), out endHours)
                && int.TryParse(ToTimeText.Substring(3, 2), out endMinutes))
            {
                var currentDate = DateTime.Now.ToUniversalTime();
                var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TaipeiStandardTime);
                var nowInGMT8 = TimeZoneInfo.ConvertTimeFromUtc(currentDate, taipeiTimeZone);

                EndTime = nowInGMT8.Date.AddHours(endHours).AddMinutes(endMinutes);
                if (EndTime > nowInGMT8)
                {
                    CountdownTime = EndTime - nowInGMT8;
                    return true;
                }
            }

            return false;
        }
    }
}