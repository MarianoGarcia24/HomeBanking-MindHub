using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account GetAccountById(long id)
        {
            Account ac = _accountRepository.FindById(id);
            if (ac == null)
            {
                throw new NullReferenceException();
            }
            return ac;
        }

        public IEnumerable<Account> GetAccountsByClient(long id)
        {
            IEnumerable<Account> accs = _accountRepository.FindAccountsByClient(id);
            if (accs.IsNullOrEmpty())
                throw new NullReferenceException();
            return accs;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            IEnumerable<Account> accs = _accountRepository.GetAll();
            if (accs.IsNullOrEmpty())
                throw new NullReferenceException();
            return accs;
        }

        public void SaveAccount(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
