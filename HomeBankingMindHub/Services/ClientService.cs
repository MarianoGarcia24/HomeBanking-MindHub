using HomeBankingMindHub.DTOs;
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

        private bool ValidateEntries(ClientSignUpDTO signUpDTO)
        {
            if (String.IsNullOrEmpty(signUpDTO.Email) || String.IsNullOrEmpty(signUpDTO.Password) ||
                   String.IsNullOrEmpty(signUpDTO.FirstName) || String.IsNullOrEmpty(signUpDTO.LastName))
                return false;
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (_clientRepository.FindByEmail(email) != null)
                return false;
            return true;
        }


        public Client CreateClient(ClientSignUpDTO signUpDTO)
        {
            //Valido los datos de entrada
            if (!ValidateEntries(signUpDTO))
                throw new ArgumentException("Datos de creacion invalidos. Corrija los errores y reintente nuevamente");
            //Valido si el email no esta en uso
            if (!ValidateEmail(signUpDTO.Email))
                throw new InvalidOperationException("El mail ya se encuentra en uso. Pruebe con uno nuevo");
           
            //Lo creo
            Client cl = new Client
            {
                Email = signUpDTO.Email,
                Password = signUpDTO.Password,
                FirstName = signUpDTO.FirstName,
                LastName = signUpDTO.LastName
            };
            //Llamo al repositorio para guardarlo
            SaveClient(cl);
            return cl;
        }

        public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }

    }
}
