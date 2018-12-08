using System.Threading.Tasks;
using BurnForMoney.Functions.Shared.Persistence;
using BurnForMoney.Infrastructure.Events;

namespace BurnForMoney.Functions.ReadModel
{
    public class ActivityView : IHandles<ActivityAdded>
    {
        private readonly string _sqlConnectionString;

        public ActivityView(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task HandleAsync(ActivityAdded message)
        {
            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();



            }
        }
    }
}