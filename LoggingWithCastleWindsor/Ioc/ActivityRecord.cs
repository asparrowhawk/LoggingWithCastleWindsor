using System;

namespace LoggingWithCastleWindsor.Ioc
{
    public class ActivityRecord : IDisposable
    {
        private readonly IDisposable _disposable;

        public ActivityRecord(string name)
            : this(name, Push)
        { }

        private static IDisposable Push(string value)
        {
            var ndc = log4net.ThreadContext.Stacks["NDC"];
            return ndc.Push(value);
        }

        internal ActivityRecord(string name, Func<string, IDisposable> push)
        {
            _disposable = push(name);
        }

        public void Dispose()
        {
            using(_disposable)
            { }
        }
    }
}