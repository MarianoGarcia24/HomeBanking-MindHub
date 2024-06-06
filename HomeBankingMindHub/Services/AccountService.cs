using HomeBankingMindHub.DTOs;
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

        public string GenerateNewAccountNumber()
        {
            string acNumber;
            do
            {
                acNumber = new Random().Next(1000, 100000000).ToString();
            } while (_accountRepository.FindByAccountNumber(acNumber) != null);
            return acNumber;
            
        }

        public Account CreateNewAccount(long clientID)
        {
            var clAccounts = GetAccountsByClient(clientID);
            if (clAccounts.Count() == 3)
            {
                throw new InvalidOperationException("Numero de cuentas máximo alcanzado. El cliente posee 3 cuentas.");
            }
            string acNumber = GenerateNewAccountNumber();
            Account acc = new Account
            {
                Balance = 0,
                Number = "VIN - " + acNumber,
                ClientID = clientID,
                CreationDate = DateTime.Now,
            };
            SaveAccount(acc);
            return acc;
        }

        public Account GetAccountById(long id)
        {
            Account ac = _accountRepository.FindById(id);
            if (ac == null)
            {
                throw new NullReferenceException("No existe cuenta para el id solicitado.");
            }
            return ac;
        }

        public IEnumerable<Account> GetAccountsByClient(long id)
        {
            IEnumerable<Account> accs = _accountRepository.FindAccountsByClient(id);
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

        public IEnumerable<AccountDTO> GetAllAccountDTOs()
        {
            return GetAllAccounts().Select(acc => new AccountDTO(acc));
        }

        public AccountDTO GetAccountDTOById(long id)
        {
            return new AccountDTO(GetAccountById(id));
        }
    }
}
