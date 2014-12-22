using System.Collections.Generic;

namespace LoggingWithCastleWindsor.Domain
{
    public interface IMarket
    {
        IEnumerable<Ticker> GetAll();
        Ticker Get(string symbol);
    }
}
