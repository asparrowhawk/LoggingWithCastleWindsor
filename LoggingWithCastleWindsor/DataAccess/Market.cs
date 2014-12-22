using System;
using System.Collections.Generic;
using System.Linq;
using LoggingWithCastleWindsor.Domain;
using LoggingWithCastleWindsor.Infrastructure;

namespace LoggingWithCastleWindsor.DataAccess
{
    public class Market : IMarket
    {
        private readonly IList<Ticker> _tickers;

        public Market(IEnumerable<Ticker> tickers)
        {
            Guard.ArgumentNotNull(tickers, "tickers");

            _tickers = tickers.ToList();
        }

        public IEnumerable<Ticker> GetAll()
        {
            return _tickers;
        }

        public Ticker Get(string symbol)
        {
            return _tickers.FirstOrDefault(ticker => Compare(ticker.Symbol, symbol));
        }

        private static bool Compare(string lhs, string rhs)
        {
            return string.Compare(lhs, rhs, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}
