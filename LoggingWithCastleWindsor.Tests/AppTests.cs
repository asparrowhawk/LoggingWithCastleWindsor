using FluentAssertions;
using LoggingWithCastleWindsor.Ioc;
using NUnit.Framework;

namespace LoggingWithCastleWindsor.Tests
{
    [TestFixture]
    public class AppTests
    {
        private TestContext _context;

        [SetUp]
        public void EachTestSetup()
        {
            _context = new TestContext();
        }

        [Test]
        public void When_Constructed_Then_No_Exceptions_Thrown()
        {
            Assert.DoesNotThrow(() => _context.BuildApp());
        }

        [Test, Description(@"Review the NCrunch or R# Unit Test Sessions output window to see examples of both Explicit (App) and Implicit (IContoller, IMarket) logging:

Example output from Log4Net TraceAppender:

LoggingWithCastleWindsor.App: 2014-12-22 13:06:08,968 [8] Calling Find with Aapl
LoggingWithCastleWindsor.Presentation.Controller: 2014-12-22 13:06:08,992 [8] Calling IController.Find(String: Aapl)
LoggingWithCastleWindsor.DataAccess.Market: 2014-12-22 13:06:08,995 [8] Calling IMarket.Get(String: Aapl)
LoggingWithCastleWindsor.DataAccess.Market: 2014-12-22 13:06:08,996 [8]   IMarket.Get returned Ticker: Currency = 'Dollar', Name = 'Apple Inc', Price = '111.78', Symbol = 'AAPL'
LoggingWithCastleWindsor.App: 2014-12-22 13:06:08,999 [8] Calling List
LoggingWithCastleWindsor.Presentation.Controller: 2014-12-22 13:06:08,999 [8] Calling IController.List(Decimal: 100.0, Decimal: 550.0)
LoggingWithCastleWindsor.DataAccess.Market: 2014-12-22 13:06:09,000 [8] Calling IMarket.GetAll()
LoggingWithCastleWindsor.DataAccess.Market: 2014-12-22 13:06:09,000 [8]   IMarket.GetAll returned List<Ticker>: Ticker[3]
")]
        public void When_Run_Called_Then_No_Exceptions_Thrown()
        {
            // Arrange
            var app = _context.BuildApp();

            // Act
            app.Run();
        }

        class TestContext
        {
            private readonly AppIoc _ioc;

            public TestContext()
            {
                _ioc = new AppIoc();
            }

            public App BuildApp()
            {
                return _ioc.Resolve<App>();
            }
        }
    }
}
