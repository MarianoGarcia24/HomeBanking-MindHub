using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAll();
        Loan GetById(long id);
        void Save(Loan loan);
    }
}
