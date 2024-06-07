using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable <Account> GetAccountsByClient(long id);
        void SaveAccount(Account account);
        Response GetAllAccounts();
        AccountClientDTO CreateNewAccount(long clientID);
        Response GetAccountDTOById(long id);
    }
}
