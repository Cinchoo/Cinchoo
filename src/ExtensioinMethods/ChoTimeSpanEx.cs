namespace System
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public enum ChoTimeSpanFormat
    {
        Default,
        Ticks,
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days
    }

    public static class ChoTimeSpanEx
    {
        #region Shared Members (Public)

        public static string ToString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString();
        }

        public static string ToString(this TimeSpan timeSpan, ChoTimeSpanFormat format)
        {
            return Format(timeSpan, format);
        }

        public static string ToString(this TimeSpan timeSpan, string format)
        {
            if (format.IsNullOrEmpty())
                return ToString(timeSpan);
            else
                return (DateTime.Today + timeSpan).ToString(format);
        }

        public static string ToString(this TimeSpan timeSpan, IFormatProvider provider)
        {
            if (provider == null)
                return ToString(timeSpan);
            else
                return (DateTime.Today + timeSpan).ToString(provider);
        }

        public static string ToString(this TimeSpan timeSpan, string format, IFormatProvider provider)
        {
            return (DateTime.Today + timeSpan).ToString(format, provider);
        }

        #endregion Shared Members (Public)

        #region Instance Members (Private)

        private static string Format(TimeSpan timeSpan, ChoTimeSpanFormat format)
        {
            switch (format)
            {
                case ChoTimeSpanFormat.Ticks:
                    return timeSpan.Ticks.ToString();
                case ChoTimeSpanFormat.Milliseconds:
                    return timeSpan.TotalMilliseconds.ToString();
                case ChoTimeSpanFormat.Seconds:
                    return timeSpan.TotalSeconds.ToString();
                case ChoTimeSpanFormat.Minutes:
                    return timeSpan.TotalMinutes.ToString();
                case ChoTimeSpanFormat.Hours:
                    return timeSpan.TotalHours.ToString();
                case ChoTimeSpanFormat.Days:
                    return timeSpan.TotalDays.ToString();
                default:
                    return timeSpan.ToString();
            }

        }

        #endregion Instance Members (Private)
    }
}
