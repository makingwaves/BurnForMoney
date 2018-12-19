using System.Threading.Tasks;
using BurnForMoney.Domain.Commands;

namespace BurnForMoney.Domain.CommandHandlers
{
    public interface ICommandHandler<in T> where T: Command
    {
        Task HandleAsync(T message);
    }
}