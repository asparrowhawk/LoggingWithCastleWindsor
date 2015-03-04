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
                _performanceCounter.NextValue();
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to create performance counter with path: " + path, exception);
            }
        }

        public override string ToString()
        {
            var value = _performanceCounter.NextValue();

            return (_convert == null)
                ? Convert.ToString(value)
                : _convert(value);
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