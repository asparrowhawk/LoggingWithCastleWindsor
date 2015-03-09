using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Ioc
{
    internal class LoggingInstaller<TApplicationType> : IWindsorInstaller
        where TApplicationType : class
    {
        private readonly LoggingContext _loggingContext;

        public LoggingInstaller()
            : this((key, value) => log4net.GlobalContext.Properties[key] = value)
        { }

        internal LoggingInstaller(Action<string, object> addToConext)
        {
            _loggingContext = new LoggingContext(addToConext, typeof(TApplicationType).Assembly);
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            _loggingContext.AddGlobalProperties();

            container.AddLoggingFacility("log4net.config");

            container.Install(
                new InterceptorInstaller<LoggingInterceptor>(),
                new InterceptorInstaller<ExceptionInterceptor>(),
                new InterceptorInstaller<TimingInterceptor>(),
                new InterceptorInstaller<ActivityInterceptor>()
                );
        }
    }
}
