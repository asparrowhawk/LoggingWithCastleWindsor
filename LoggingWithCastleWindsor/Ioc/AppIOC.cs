﻿using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using LoggingWithCastleWindsor.DataAccess;
using LoggingWithCastleWindsor.Presentation;

namespace LoggingWithCastleWindsor.Ioc
{
    public class AppIoc
    {
        private readonly WindsorContainer _container;

        public AppIoc()
        {
            _container = new WindsorContainer();

            ConfigureFacilities(_container);

            _container.Install(
                new LoggingInstaller<App>(),
                new DataAccessInstaller(),
                new PresentationInstaller()
                );

            _container.Register(
                Component.For<App>()
                );
        }

        private static void ConfigureFacilities(WindsorContainer container)
        {
            container.AddFacility<CollectionResolverFacility>();
            container.AddFacility<InterceptorSelectorFacility<InterceptorsSelector>>();
            container.AddFacility<TypedFactoryFacility>();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
