using System.Threading.Tasks;
using BurnForMoney.Domain;

namespace BurnForMoney.ReadModel
{
    public interface IHandles<in T> where T: DomainEvent
    {
        Task HandleAsync(T message);
    }
}