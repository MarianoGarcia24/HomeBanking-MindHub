using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> getAllAccounts();
        Account GetById(long id);
        void Save(Account account);
    }
}
