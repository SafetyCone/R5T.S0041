using System;
using System.Globalization;

using R5T.T0132;


namespace R5T.S0041
{
    [DraftFunctionalityMarker]
    public interface IDateOperator : IDraftFunctionalityMarker
    {
        public DateTime From_YYYYMMDD(string yyyymmdd)
        {
            var output = DateTime.ParseExact(yyyymmdd, "yyyyMMdd", CultureInfo.InvariantCulture);
            return output;
        }

        public DateTime GetDefault()
        {
            var output = default(DateTime);
            return output;
        }

        public DateTime GetMaximum()
        {
            var output = DateTime.MaxValue;
            return output;
        }

        public DateTime GetMinimum()
        {
            var output = DateTime.MinValue;
            return output;
        }

        public DateTime GetNow_Local()
        {
            var output = DateTime.Now;
            return output;
        }

        public DateTime GetNow_UTC()
        {
            var output = DateTime.UtcNow;
            return output;
        }

        /// <summary>
        /// Chooses <see cref="GetNow_Local"/> as the default.
        /// </summary>
        public DateTime GetNow()
        {
            var output = this.GetNow_Local();
            return output;
        }

        public DateTime GetDay(DateTime now)
        {
            var output = new DateTime(
                now.Year,
                now.Month,
                now.Day);

            return output;
        }

        public DateTime GetToday_Local()
        {
            var now = this.GetNow_Local();

            var today = this.GetDay(now);
            return today;
        }

        public DateTime GetToday_UTC()
        {
            var now = this.GetNow_Local();

            var today = this.GetDay(now);
            return today;
        }

        /// <summary>
        /// Chooses <see cref="GetToday_Local"/> as the default.
        /// </summary>
        public DateTime GetToday()
        {
            var output = this.GetToday_Local();
            return output;
        }

        public DateTime GetYesterday()
        {
            var today = this.GetToday();

            var yesterday = today.AddDays(-1);
            return yesterday;
        }

        public bool IsDefault(DateTime dateTime)
        {
            var output = dateTime == default;
            return output;
        }

        public string ToString_YYYYMMDD(DateTime dateTime)
        {
            var output = $"{dateTime:yyyyMMdd}";
            return output;
        }
    }
}
