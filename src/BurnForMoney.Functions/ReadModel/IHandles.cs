using System.Threading.Tasks;
using BurnForMoney.Infrastructure;

namespace BurnForMoney.Functions.ReadModel
{
    public interface IHandles<in T> where T: DomainEvent
    {
        Task HandleAsync(T message);
    }
}