using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Commands;

namespace BurnForMoney.Infrastructure.CommandHandlers
{
    public interface ICommandHandler<in T> where T: Command
    {
        Task HandleAsync(T message);
    }
}