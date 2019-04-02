using System;
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

        public async Task<bool> DispatchActivityEvent(DomainEvent @event)
        {
            switch (@event)
            {
                case ActivityAdded activityAdded:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityAdded);
                    return true;
                case ActivityUpdated_V2 activityUpdated:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityUpdated);
                    return true;
                case ActivityDeleted_V2 activityDeleted:
                    await new RankingView(_sqlDbConnectionString).HandleAsync(activityDeleted);
                    return true;
                default:
                    return false;
            }
        }

        public async Task<bool> DispatchAthleteEvent(DomainEvent @event)
        {
            switch (@event)
            {
                 case AthleteCreated created:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(created);
                    return true;
                case AthleteDeactivated deactivated:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(deactivated);
                    return true;
                case AthleteActivated activated:
                    await new AthleteView(_sqlDbConnectionString).HandleAsync(activated);
                    return true;
                case ActivityAdded activityAdded:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityAdded);
                    return true;
                case ActivityUpdated_V2 activityUpdated:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityUpdated);
                    return true;
                case ActivityDeleted_V2 activityDeleted:
                    await new ActivityView(_sqlDbConnectionString, _log).HandleAsync(activityDeleted);
                    return true;
                case PointsGranted _:
                    return true;
                case PointsLost _:
                    return true;
                default:
                    return false;
            }
        }
    }
}