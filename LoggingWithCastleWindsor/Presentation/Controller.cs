using System.Collections.Generic;
using System.Linq;
using LoggingWithCastleWindsor.Domain;
using LoggingWithCastleWindsor.Infrastructure;

namespace LoggingWithCastleWindsor.Presentation
{
    public class Controller : IController
    {
        private readonly IMarket _market;

        public Controller(IMarket market)
        {
            Guard.ArgumentNotNull(market, "market");

            _market = market;
        }

        public void Find(string symbol)
        {
            Result = _market.Get(symbol);
        }

        public void List(decimal min, decimal max)
        {
            Results = _market.GetAll()
                .Where(ticker => ticker.Price >= min && ticker.Price <= max);
        }

        public Ticker Result { get; private set; }

        public IEnumerable<Ticker> Results { get; private set; }
    }
}
