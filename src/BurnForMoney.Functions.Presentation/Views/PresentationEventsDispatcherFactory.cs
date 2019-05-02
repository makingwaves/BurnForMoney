using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Presentation.Views
{
    public interface IPresentationEventsDispatcherFactory
    {
        IPresentationEventsDispatcher Create(ILogger logger);
    }

    public class PresentationEventsDispatcherFactory : IPresentationEventsDispatcherFactory
    {
        private readonly string _sqlDbConnectionString;

        public PresentationEventsDispatcherFactory(string sqlDbConnectionString)
        {
            _sqlDbConnectionString = sqlDbConnectionString;
        }

        public IPresentationEventsDispatcher Create(ILogger logger)
        {
            return new PresentationEventsDispatcher(_sqlDbConnectionString, logger);
        }
    }
}