using System;
using System.Diagnostics;

namespace LoggingWithCastleWindsor.Ioc
{
    class PerformanceCounterSnapshot
    {
        private readonly Func<float, string> _convert;
        private readonly PerformanceCounter _performanceCounter;

        public PerformanceCounterSnapshot(string path, Func<float, string> convert = null)
        {
            _convert = convert;
            _performanceCounter = CreatePerformanceCounter(path);
            try
            {
                NextValue();
            }
            catch (Exception exception)
            {
                var message = string.Format("Unable to create performance counter with path: '{0}'", path);
                throw new Exception(message, exception);
            }
        }

        private float NextValue()
        {
            return _performanceCounter.NextValue();
        }

        public override string ToString()
        {
            try
            {
                var value = NextValue();

                return (_convert == null)
                    ? Convert.ToString(value)
                    : _convert(value);
            }
            catch(Exception exception)
            {
                return exception.Message;
            }
        }

        private static PerformanceCounter CreatePerformanceCounter(string path)
        {
            var parts = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            return (parts.Length > 2)
                ? new PerformanceCounter(parts[0], parts[1], parts[2], true)
                : new PerformanceCounter(parts[0], parts[1], true);
        }
    }
}