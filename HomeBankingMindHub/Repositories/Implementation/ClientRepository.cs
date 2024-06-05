using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class ClientRepository : RepositoryBase<Client>, IClientRepository
    {
        public ClientRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Client> GetAll()
        {
            return FindAll()
                .Include(client => client.Accounts)
                .Include(client => client.ClientLoans)
                .ThenInclude(clientLoan => clientLoan.Loan)
                .Include(client => client.Cards)
                .ToList();
        }

        public Client FindByEmail(string email)
        {
            return FindByCondition(cl => cl.Email.ToUpper() == email.ToUpper())
                .Include(cl => cl.Accounts)
                .Include(cl => cl.ClientLoans)
                    .ThenInclude(cl => cl.Loan)
                .Include(cl => cl.Cards)
                .FirstOrDefault();
        }

        public Client FindById(long id)
        {
            return FindByCondition(client => client.Id == id)
                .Include(client => client.Accounts)
                .Include(client => client.ClientLoans)
                .ThenInclude(clientLoan => clientLoan.Loan)
                .Include(client => client.Cards)
                .FirstOrDefault();
        }


        public void Save(Client client)
        {
            Create(client);
            SaveChanges();
        }

        public void UpdateClient(Client client)
        {
            Update(client);
            SaveChanges();
        }

        
    }
}
