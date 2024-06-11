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

    }
}
