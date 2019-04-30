using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class CreateAthleteCommandHandler : ICommandHandler<CreateAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;
        private readonly IAccountsStore _accountsStore;

        public CreateAthleteCommandHandler(IRepository<Athlete> repository, IAccountsStore accountsStore)
        {
            _repository = repository;
            _accountsStore = accountsStore;
        }

        public async Task HandleAsync(CreateAthleteCommand message)
        {
            if (await _accountsStore.GetAccountByActiveDirectoryId(message.AadId) != null)
            {
                throw new ConcurrencyException();
            }

            try
            {
                if (!await _accountsStore.TryCreateAccount(new AccountEntity(message.Id, message.AadId)))
                {
                    throw new FailedToAddAthleteException(message.Id.ToString("D"), message.AadId.ToString("D"));
                }
                var athlete = new Athlete(message.Id, message.AadId, message.FirstName, message.LastName);
                await _repository.SaveAsync(athlete, 0);
            }
            catch (Exception)
            {
                await _accountsStore.DeleteIfExistsAsync(message.AadId);
                throw;
            }
        }
    }
}