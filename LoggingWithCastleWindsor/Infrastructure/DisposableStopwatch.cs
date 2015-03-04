using System;
using System.Diagnostics;
using Castle.Core.Logging;

namespace LoggingWithCastleWindsor.Infrastructure
{
    public class DisposableStopwatch : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _activity;
        private readonly Stopwatch _stopwatch;

        public DisposableStopwatch(ILogger logger, string activity)
        {
            _logger = logger;
            _activity = activity;
            _stopwatch = new Stopwatch();

            InfoFormat("Starting '{0}'", activity);

            _stopwatch.Start();
        }

        private void InfoFormat(string format, params object[] args)
        {
            if (_logger.IsInfoEnabled)
            {
                _logger.InfoFormat(format, args);
            }
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            InfoFormat("Completed '{0}' in {1} ms", _activity, _stopwatch.ElapsedMilliseconds);
        }
    }
}
