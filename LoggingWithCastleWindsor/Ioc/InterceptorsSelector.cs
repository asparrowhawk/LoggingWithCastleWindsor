using System;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Proxy;

namespace LoggingWithCastleWindsor.Ioc
{
    internal class InterceptorsSelector : IModelInterceptorsSelector
    {
        public bool HasInterceptors(ComponentModel model)
        {
            return model.Services.Any(MatchesNamespace);
        }

        private static bool MatchesNamespace(Type service)
        {
            return service.Namespace == "LoggingWithCastleWindsor.Domain" ||
                   service.Namespace == "LoggingWithCastleWindsor.Presentation";
        }

        public InterceptorReference[] SelectInterceptors(
            ComponentModel model, InterceptorReference[] interceptors
            )
        {
            // Implicit logging ...
            return new[]
                {
                    InterceptorReference.ForType<LoggingInterceptor>(),
                    InterceptorReference.ForType<ExceptionInterceptor>(),
                    InterceptorReference.ForType<TimingInterceptor>()
                };
        }
    }
}
