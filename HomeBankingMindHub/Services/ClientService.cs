using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public IEnumerable<Client> GetAll()
        {
            return _clientRepository.GetAll();
        }

        public Client GetClientByEmail(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl == null) 
                throw new NullReferenceException();
            return cl;
        }

        public Client GetClientById(long clientId)
        {
            Client cl = _clientRepository.FindById(clientId);
            if (cl == null)
                throw new NullReferenceException("No existe el cliente solicitado");
            return cl;
        }

        

    }
}
