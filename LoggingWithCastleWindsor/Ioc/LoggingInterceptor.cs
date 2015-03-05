using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using LoggingWithCastleWindsor.Infrastructure;

namespace LoggingWithCastleWindsor.Ioc
{
    public class BaseInterceptor : IInterceptor
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDictionary<Type, ILogger> _loggers;

        protected BaseInterceptor(ILoggerFactory loggerFactory)
            : this(loggerFactory, new Dictionary<Type, ILogger>())
        { }

        protected BaseInterceptor(ILoggerFactory loggerFactory, IDictionary<Type, ILogger> loggers)
        {
            Guard.ArgumentNotNull(loggerFactory, "loggerFactory");
            Guard.ArgumentNotNull(loggers, "loggers");

            _loggerFactory = loggerFactory;
            _loggers = loggers;
        }

        public void Intercept(IInvocation invocation)
        {
            var logger = GetLogger(invocation.TargetType);

            Intercept(logger, invocation);
        }

        protected virtual void Intercept(ILogger logger, IInvocation invocation)
        {
            invocation.Proceed();
        }

        private ILogger GetLogger(Type type)
        {
            ILogger result;
            if (_loggers.TryGetValue(type, out result))
            {
                return result;
            }

            var logger = _loggerFactory.Create(type);
            _loggers.Add(type, logger);

            return logger;
        }
    }

    public class ActivityInterceptor : BaseInterceptor
    {
        public ActivityInterceptor(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }

        protected override void Intercept(ILogger logger, IInvocation invocation)
        {
            var activity = GetActivityOrDefault(invocation);

            if (activity != null)
            {
                using (new ActivityRecord(activity.Name))
                {
                    base.Intercept(logger, invocation);
                    return;
                }
            }

            base.Intercept(logger, invocation);
        }

        private static ActivityAttribute GetActivityOrDefault(IInvocation invocation)
        {
            var type = typeof (ActivityAttribute);

            var method = invocation.Method;

            var activity = method.GetCustomAttributes(type)
                .Concat(method.DeclaringType.GetCustomAttributes(type))
                .FirstOrDefault() as ActivityAttribute;

            return activity;
        }
    }

    public class LoggingInterceptor : BaseInterceptor
    {
        public LoggingInterceptor(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }

        protected override void Intercept(ILogger logger, IInvocation invocation)
        {
            LogCall(logger, invocation);

            invocation.Proceed();

            LogReturn(logger, invocation.Method, invocation.ReturnValue);
        }

        private static void LogCall(ILogger logger, IInvocation invocation)
        {
            if (!logger.IsDebugEnabled)
            {
                return;
            }

            var method = invocation.Method;

            var arguments = invocation.Arguments;

            LogDebug(logger, "Calling {0}", method.NameAndArguments(arguments));
        }

        private static void LogDebug(ILogger logger, string format, params object[] args)
        {
            logger.DebugFormat(
                new TypeAndPropertiesFormatter(), 
                format,
                args
                );
        }

        private static void LogReturn(ILogger logger, MethodInfo method, object returnValue)
        {
            if (logger.IsDebugEnabled && method.ReturnType != typeof(void))
            {
                LogDebug(logger, "  {0} returned {1:TP}", method.DeclaringTypeAndName(), returnValue);
            }
        }
    }

    public class ExceptionInterceptor : BaseInterceptor
    {
        public ExceptionInterceptor(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }

        protected override void Intercept(ILogger logger, IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception exception)
            {
                LogException(logger, exception, invocation.Method);

                throw;
            }
        }

        private static void LogException(ILogger logger, Exception exception, MethodInfo method)
        {
            if (logger.IsErrorEnabled)
            {
                logger.ErrorFormat(exception, method.DeclaringTypeAndName());
            }
        }
    }

    public class TimingInterceptor : BaseInterceptor
    {
        public TimingInterceptor(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        { }

        protected override void Intercept(ILogger logger, IInvocation invocation)
        {
            using(new DisposableStopwatch(logger, invocation.Method.DeclaringTypeAndName()))
            {
                invocation.Proceed();
            }
        }
    }
}
