using Castle.Facilities.Logging;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Ioc
{
    public static class WindsorExtensions
    {
        public static void AddLoggingFacility(this IWindsorContainer container, string configFile)
        {
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(LoggerImplementation.Log4net).WithConfig(configFile)
                );
        }
    }
}
