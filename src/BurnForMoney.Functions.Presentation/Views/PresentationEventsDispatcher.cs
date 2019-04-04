using System.Threading.Tasks;
using BurnForMoney.Domain;
using BurnForMoney.Domain.Events;
using Microsoft.Extensions.Logging;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class PresentationEventsDispatcher
    {
        private readonly string _sqlDbConnectionString;
        private readonly ILogger _log;

        public PresentationEventsDispatcher(string sqlDbConnectionString, ILogger log)
        {
            _sqlDbConnectionString = sqlDbConnectionString;
            _log = log;
        }

        public async Task DispatchActivityEvent(DomainEvent @event)
        {
            switch (@event)
            {
                case ActivityAdded activityAdded:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityAdded);
                    await new MonthlyResultsView(_sqlDbConnectionString).HandleAsync(activityAdded);
                    break;
                case ActivityUpdated_V2 activityUpdated:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityUpdated);
                    await new MonthlyResultsView(_sqlDbConnectionString).HandleAsync(activityUpdated);
                    break;
                case ActivityDeleted_V2 activityDeleted:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityDeleted);
                    await new MonthlyResultsView(_sqlDbConnectionString).HandleAsync(activityDeleted);
                    break;
            }
        }

        public async Task DispatchAthleteEvent(DomainEvent @event)
        {
            switch (@event)
            {
                case AthleteCreated_V2 created:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(created);
                    break;
                case AthleteDeactivated deactivated:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(deactivated);
                    break;
                case AthleteActivated activated:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(activated);
                    break;
                case ActivityAdded activityAdded:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityAdded);
                    break;
                case ActivityUpdated_V2 activityUpdated:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityUpdated);
                    break;
                case ActivityDeleted_V2 activityDeleted:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityDeleted);
                    break;
            }
        }
    }
}