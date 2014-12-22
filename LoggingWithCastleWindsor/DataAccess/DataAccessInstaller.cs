using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using LoggingWithCastleWindsor.Domain;

namespace LoggingWithCastleWindsor.DataAccess
{
    internal class DataAccessInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Classes.FromThisAssembly()
                    .InNamespace("LoggingWithCastleWindsor.DataAccess")
                    .WithService.DefaultInterfaces(),

                Component.For<IEnumerable<Ticker>>().UsingFactoryMethod(
                    () => new[]
                        {
                            new Ticker { Currency = Currency.Dollar, Name = "Apple Inc", Price = 111.78m, Symbol = "AAPL" },
                            new Ticker { Currency = Currency.Dollar, Name = "Google Inc", Price = 516.35m, Symbol = "GOOG" },
                            new Ticker { Currency = Currency.Dollar, Name = "Microsoft Corp", Price = 47.66m, Symbol = "MSFT" }
                        })

                );
        }
    }
}
