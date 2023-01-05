namespace Fanex.Alpha.Before.Models
{
    public class UnderMaintenanceModel
    {
        private const string TaipeiStandardTime = "Taipei Standard Time";
        private readonly DateTime utcNow;

        public UnderMaintenanceModel(
            bool isUm,
            string fromTimeText,
            string toTimeText,
            bool hasError,
            DateTime? utcNow = null)
        {
            IsUM = isUm;
            FromTimeText = fromTimeText;
            ToTimeText = toTimeText;
            HasError = hasError;
            this.utcNow = utcNow != null ? utcNow.Value : DateTime.UtcNow;
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
                || (ToTimeText.Length > 2 && !ToTimeText.Contains(":")))
            {
                return false;
            }

            var doubleColonIndex = ToTimeText.IndexOf(':');

            if (int.TryParse(ToTimeText.Substring(0, doubleColonIndex), out int endHours)
                && int.TryParse(ToTimeText.Substring(doubleColonIndex + 1, 2), out int endMinutes))
            {
                if (ToTimeText.ToUpperInvariant().Contains("PM") && endHours < 12)
                {
                    endHours += 12;
                }

                var taipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById(TaipeiStandardTime);
                var nowInGMT8 = TimeZoneInfo.ConvertTimeFromUtc(utcNow, taipeiTimeZone);

                EndTime = nowInGMT8.Date.AddHours(endHours).AddMinutes(endMinutes + 2);
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