using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace LoggingWithCastleWindsor.Presentation
{
    internal class PresentationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Classes.FromThisAssembly()
                    .InNamespace("LoggingWithCastleWindsor.Presentation")
                    .WithService.DefaultInterfaces()

                );
        }
    }
}
