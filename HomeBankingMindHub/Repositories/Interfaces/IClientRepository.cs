using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAll();
        void Save(Client client);
        void UpdateClient(Client client);
        Client FindById(long id);
        Client FindByEmail(string email);
    }
}
