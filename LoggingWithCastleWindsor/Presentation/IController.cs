using LoggingWithCastleWindsor.Ioc;

namespace LoggingWithCastleWindsor.Presentation
{
    public interface IController
    {
        [Activity("Finding symbol")]
        void Find(string symbol);

        [Activity("Listing symbols between value range")]
        void List(decimal min, decimal max);
    }
}