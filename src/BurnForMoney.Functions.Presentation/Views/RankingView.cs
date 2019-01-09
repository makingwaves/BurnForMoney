using System.Threading.Tasks;
using BurnForMoney.Domain.Events;

namespace BurnForMoney.Functions.Presentation.Views
{
    public class RankingView : IHandles<ActivityAdded>, IHandles<ActivityDeleted>, IHandles<ActivityUpdated>
    {
        private readonly string _sqlConnectionString;

        public RankingView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public Task HandleAsync(ActivityAdded message)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(ActivityDeleted message)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(ActivityUpdated message)
        {
            return Task.CompletedTask;
        }
    }
}