using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Services.Implementations
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

        public AccountClientDTO CreateNewAccount(long clientID)
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
                Number = "VIN" + acNumber,
                ClientID = clientID,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);
            Account acc2 = _accountRepository.FindByAccountNumber(acc.Number);
            return new AccountClientDTO(acc2);
        }

        private Response GetAccountById(long id)
        {
            Account ac = _accountRepository.FindById(id);
            if (ac == null)
            {
                return new Response(System.Net.HttpStatusCode.NotFound, "No se encontro la cuenta solicitada: " + id);
            }
            return new Response(System.Net.HttpStatusCode.OK, ac);
        }

        public IEnumerable<Account> GetAccountsByClient(long id)
        {
            IEnumerable<Account> accs = _accountRepository.FindAccountsByClient(id);
            return accs;
        }

        public Response GetAllAccounts()
        {
            IEnumerable<Account> accs = _accountRepository.GetAll();
            return new Response(System.Net.HttpStatusCode.OK, accs);
        }

        public Response GetAccountDTOById(long id)
        {
            Response res = GetAccountById(id);
            if (res.StatusCode == 200)
            {
                AccountDTO newAcc = new AccountDTO((Account)res.Data);
                res.Data = newAcc;
                return res;
            }
            return res;
        }

    }
}
