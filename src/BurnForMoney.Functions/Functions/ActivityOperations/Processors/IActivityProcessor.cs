using BurnForMoney.Functions.Functions.ActivityOperations.Dto;
using BurnForMoney.Functions.Shared.Queues;

namespace BurnForMoney.Functions.Functions.ActivityOperations
{
    public interface IActivityProcessor
    {
        bool CanProcess(PendingRawActivity raw);
        PendingActivity Process(PendingRawActivity raw);
    }
}