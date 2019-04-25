using System;
using System.Threading.Tasks;
using BurnForMoney.Functions.Commands;
using BurnForMoney.Functions.Domain;
using BurnForMoney.Functions.Exceptions;
using BurnForMoney.Infrastructure.Persistence;

namespace BurnForMoney.Functions.CommandHandlers
{
    public class AssignActiveDirectoryIdToAthleteCommandHandler : ICommandHandler<AssignActiveDirectoryIdToAthleteCommand>
    {
        private readonly IRepository<Athlete> _repository;
        private readonly IAccountsStore _accountsStore;

        public AssignActiveDirectoryIdToAthleteCommandHandler(IRepository<Athlete> repository, IAccountsStore accountsStore)
        {
            _repository = repository;
            _accountsStore = accountsStore;
        }

        public async Task HandleAsync(AssignActiveDirectoryIdToAthleteCommand message)
        {
            Athlete athlete = await _repository.GetByIdAsync(message.AthleteId);

            if (athlete == null)
            {
                throw new FailedToCreateAccountException($"Athlete: [{message.AthleteId}] not found");
            }

            AccountEntity account = await _accountsStore.GetAccountById(message.AthleteId);
            if (account != null)
            {
                throw new InvalidOperationException("Athlete already has an account");
            }

            if (await _accountsStore.TryCreateAccount(new AccountEntity(message.AthleteId, message.ActiveDirectoryId)))
            {
                athlete.AssignActiveDirectoryId(message.ActiveDirectoryId);
                await _repository.SaveAsync(athlete, athlete.OriginalVersion);
            } else
            {
                throw new FailedToCreateAccountException(message.AthleteId.ToString("D"), message.ActiveDirectoryId.ToString("D"));
            }
        }
    }
}