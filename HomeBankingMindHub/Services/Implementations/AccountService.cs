using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;

        public AccountService(IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
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

        public Response GetAllAccounts()
        {
            IEnumerable<AccountDTO> accs = _accountRepository.GetAll().Select(c => new AccountDTO(c));
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

        public Response CreateNewAccount(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            var clAccounts = _accountRepository.FindAccountsByClient(cl.Id);
            if (clAccounts.Count() == 3)
            {
                throw new InvalidOperationException("Numero de cuentas máximo alcanzado. El cliente posee 3 cuentas.");
            }

            string acNumber = GenerateNewAccountNumber();
            Account acc = new Account
            {
                Balance = 0,
                Number = "VIN" + acNumber,
                ClientID = cl.Id,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);
            Account acc2 = _accountRepository.FindByAccountNumber(acc.Number);
            return new Response(HttpStatusCode.Created, new AccountDTO(acc2));
        }

        private string GenerateNewAccountNumber()
        {
            string acNumber;
            do
            {
                acNumber = new Random().Next(1000, 100000000).ToString();
            } while (_accountRepository.FindByAccountNumber(acNumber) != null);
            return acNumber;
        }


    }
}
