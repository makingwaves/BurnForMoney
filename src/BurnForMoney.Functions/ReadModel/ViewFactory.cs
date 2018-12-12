using BurnForMoney.Functions.Configuration;
using BurnForMoney.Infrastructure;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Functions.ReadModel
{
    public class ViewFactory
    {
        private readonly ConfigurationRoot _configuration;

        public ViewFactory(ConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public IHandles<T> GetFor<T>(T domainEvent) where T: DomainEvent
        {
            IHandles<T> handler = null;
            if (domainEvent is AthleteCreated || domainEvent is AthleteDeactivated)
            {
                handler = (IHandles<T>)new AthleteView(_configuration.ConnectionStrings.SqlDbConnectionString);
            }
            else if (domainEvent is ActivityAdded || domainEvent is ActivityUpdated || domainEvent is ActivityDeleted)
            {
                handler = (IHandles<T>)new ActivityView(_configuration.ConnectionStrings.SqlDbConnectionString);
            }

            return handler;
        }
    }
}