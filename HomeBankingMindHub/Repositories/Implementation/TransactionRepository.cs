using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Transaction> GetAll()
        {
            return FindAll()
                .ToList();
        }

        public Transaction FindById(long id)
        {
            return FindByCondition(transaction => transaction.Id == id)
                    .FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
            RepositoryContext.ChangeTracker.Clear();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return RepositoryContext.Database.BeginTransaction();
        }
    }
}
