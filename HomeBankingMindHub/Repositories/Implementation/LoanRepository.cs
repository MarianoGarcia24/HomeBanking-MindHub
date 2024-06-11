using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll()
                .ToList();
        }

        public Loan FindById(long id)
        {
            return FindByCondition(lo => lo.Id == id)
                .Include(lo => lo.ClientLoans)
                .FirstOrDefault();
        }

        public void Save(Loan loan)
        {
            Create(loan);
            SaveChanges();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return RepositoryContext.Database.BeginTransaction();
        }


    }
}
