namespace LoggingWithCastleWindsor.Presentation
{
    public interface IController
    {
        void Find(string symbol);
        void List(decimal min, decimal max);
    }
}