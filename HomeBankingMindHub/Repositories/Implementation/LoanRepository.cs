using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Loan> GetAll()
        {
            throw new NotImplementedException();
        }

        public Loan GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Save(Loan loan)
        {
            throw new NotImplementedException();
        }
    }
}
