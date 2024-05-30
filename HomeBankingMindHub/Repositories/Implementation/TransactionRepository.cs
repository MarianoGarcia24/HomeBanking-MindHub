using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

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

        public Transaction GetById(long id)
        {
            return FindByCondition(transaction => transaction.Id == id)
                    .FirstOrDefault();
        }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
