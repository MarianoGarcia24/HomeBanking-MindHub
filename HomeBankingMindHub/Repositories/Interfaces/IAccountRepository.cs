using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();
        Account FindById(long id);
        IEnumerable<Account> FindAccountsByClient(long id);
        void Save(Account account);
    }
}
