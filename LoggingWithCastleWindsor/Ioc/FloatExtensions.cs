using System;

namespace LoggingWithCastleWindsor.Ioc
{
    static class FloatExtensions
    {
        public static string AsFormattedValue(this float value, string format)
        {
            return value.ToString(format);
        }

        public static string AsGbytes(this float value, string layout, float divisor = 1073741824f)
        {
            string format = string.Format("{{0:{0}}}Gb", layout);
            return string.Format(format, value / divisor);
        }

        public static string AsMbytes(this float value, string layout, float divisor = 1048576)
        {
            string format = string.Format("{{0:{0}}}Mb", layout);
            return string.Format(format, value / divisor);
        }

        public static string AsProcessTime(this float value, string format)
        {
            return (value / Environment.ProcessorCount).ToString(format);
        }
    }
}