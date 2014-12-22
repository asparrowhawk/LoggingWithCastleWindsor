namespace LoggingWithCastleWindsor.Domain
{
    public class Ticker
    {
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
