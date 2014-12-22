using Castle.Core.Logging;
using LoggingWithCastleWindsor.Infrastructure;
using LoggingWithCastleWindsor.Presentation;

namespace LoggingWithCastleWindsor
{
    public class App
    {
        private readonly IController _controller;
        private ILogger _logger;

        public App(IController controller)
        {
            Guard.ArgumentNotNull(controller, "controller");

            _controller = controller;
        }

        public void Run()
        {
            // Explicit logging ...
            Logger.Info("Calling Find with Aapl");

            _controller.Find("Aapl");

            // Explicit logging ...
            Logger.Info("Calling List");

            _controller.List(100.0m, 550.0m);
        }

        public ILogger Logger
        {
            get { return _logger ?? NullLogger.Instance; }
            set { _logger = value; }
        }
    }
}
