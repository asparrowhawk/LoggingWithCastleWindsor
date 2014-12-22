using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Ioc
{
    public class InterceptorInstaller<TService> : IWindsorInstaller
        where TService : class
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<TService>()
                    .LifeStyle.Transient
                );
        }
    }
}
