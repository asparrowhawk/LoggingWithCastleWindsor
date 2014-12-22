using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Proxy;

namespace LoggingWithCastleWindsor.Ioc
{
    public class InterceptorSelectorFacility<T> : IFacility
        where T : IModelInterceptorsSelector, new()
    {
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.ProxyFactory.AddInterceptorSelector(new T());
        }

        public void Terminate()
        {

        }
    }
}
