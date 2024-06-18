using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetAll();
        Transaction FindById(long id);
        void Save(Transaction transaction);
        IDbContextTransaction BeginTransaction();
    }
}
