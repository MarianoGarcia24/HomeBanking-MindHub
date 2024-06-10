using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> FindAccountsByClient(long id)
        {
            return FindByCondition(acc => acc.ClientID == id)
                .Include(acc => acc.Transactions)
                .ToList();
        }

        public Account FindByAccountNumber(string accountNumber)
        {
            return FindByCondition(acc => String.Equals(accountNumber, acc.Number))
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
                 
        }

        public Account FindById(long id)
        {
            return FindByCondition(acc => acc.Id == id)
                .Include(acc => acc.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAll()
        {
            return FindAll()
                .Include(acc => acc.Transactions)
                .ToList();
        }


        public void Save(Account account)
        {
            if (account.Id == 0)
                Create(account);
            else
                Update(account);

            SaveChanges();
            RepositoryContext.ChangeTracker.Clear();
        }

    }
}
