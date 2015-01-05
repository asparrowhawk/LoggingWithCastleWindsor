using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using LoggingWithCastleWindsor.Infrastructure;

namespace LoggingWithCastleWindsor.Ioc
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDictionary<Type, ILogger> _loggers;

        public LoggingInterceptor(ILoggerFactory loggerFactory)
            : this(loggerFactory, new Dictionary<Type, ILogger>())
        { }

        internal LoggingInterceptor(ILoggerFactory loggerFactory, IDictionary<Type, ILogger> loggers)
        {
            Guard.ArgumentNotNull(loggerFactory, "loggerFactory");
            Guard.ArgumentNotNull(loggers, "loggers");

            _loggerFactory = loggerFactory;
            _loggers = loggers;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;

            var logger = GetLogger(invocation.TargetType);

            if (logger.IsInfoEnabled)
            {
                var arguments = invocation.Arguments;

                logger.InfoFormat("Calling {0}", method.NameAndArguments(arguments));
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception exception)
            {
                if (logger.IsErrorEnabled)
                {
                    logger.ErrorFormat(exception, method.DeclaringTypeAndName());
                }
                throw;
            }

            if (logger.IsInfoEnabled && method.ReturnType != typeof(void))
            {
                logger.InfoFormat(
                    new TypeAndPropertiesFormatter(),
                    "  {0} returned {1:TP}", method.DeclaringTypeAndName(), invocation.ReturnValue
                    );
            }
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
}
