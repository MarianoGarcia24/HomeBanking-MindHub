using HomeBankingMindHub.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAll();
        Loan FindById(long id);
        void Save(Loan loan);
        IDbContextTransaction BeginTransaction();

    }
}
