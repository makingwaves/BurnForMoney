using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Messages;

namespace BurnForMoney.Functions.CommandHandlers
{
    public interface ICommandHandler<in T> where T: Command
    {
        Task HandleAsync(T message);
    }
}