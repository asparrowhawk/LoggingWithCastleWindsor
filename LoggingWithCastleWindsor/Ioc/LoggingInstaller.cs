using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Ioc
{
    class LoggingInstaller : IWindsorInstaller
    {
        private readonly LoggingContext _loggingContext;

        public LoggingInstaller()
            : this((key, value) => log4net.GlobalContext.Properties[key] = value)
        { }

        internal LoggingInstaller(Action<string, object> addToConext)
        {
            _loggingContext = new LoggingContext(addToConext);
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            _loggingContext.AddGlobalProperties();

            container.AddLoggingFacility("log4net.config");

            container.Install(
                new InterceptorInstaller<LoggingInterceptor>(),
                new InterceptorInstaller<ExceptionInterceptor>(),
                new InterceptorInstaller<TimingInterceptor>()
                );
        }
    }
}
