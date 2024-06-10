using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
        }


        public IEnumerable<ClientDTO> GetAll()
        {
            return _clientRepository.GetAll().Select(c => new ClientDTO(c)).ToList();
        }

        public Response GetClientByEmail(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl == null)
                return new Response(HttpStatusCode.NotFound, "El cliente no existe en la base de datos");
            return new Response(HttpStatusCode.OK, new ClientDTO(cl));
        }

        private Client FindClientByEmail(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl == null)
                throw new NullReferenceException("El cliente no existe en la base de datos");
            return cl;

        }


        public Response GetClientById(long clientId)
        {
            Client cl = _clientRepository.FindById(clientId);
            if (cl == null)
                return new Response(HttpStatusCode.BadRequest, "No se encontro el cliente solicitado");
            return new Response(HttpStatusCode.OK, new ClientDTO(cl));
        }

        private bool ValidateEntries(ClientSignUpDTO signUpDTO)
        {
            if (string.IsNullOrEmpty(signUpDTO.Email) || string.IsNullOrEmpty(signUpDTO.Password) ||
                   string.IsNullOrEmpty(signUpDTO.FirstName) || string.IsNullOrEmpty(signUpDTO.LastName))
                return false;
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (_clientRepository.FindByEmail(email) != null)
                return false;
            return true;
        }


        public Response CreateClient(ClientSignUpDTO signUpDTO)
        {
            //Valido los datos de entrada
            if (!ValidateEntries(signUpDTO))
                return new Response(HttpStatusCode.BadRequest, "Datos de creacion invalidos. Corrija los errores y reintente nuevamente");
            //Valido si el email no esta en uso
            if (!ValidateEmail(signUpDTO.Email))
                return new Response(HttpStatusCode.Forbidden, "El mail ya se encuentra en uso. Pruebe con uno nuevo");

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
            cl = FindClientByEmail(cl.Email);
            return new Response(HttpStatusCode.Created, cl);
        }

        public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }

        public Response ValidateCredentials(ClientLoginDTO clientLoginDTO)
        {
            Client cl = FindClientByEmail(clientLoginDTO.Email);
            if (cl == null || !string.Equals(clientLoginDTO.Password, cl.Password))
                return new Response(HttpStatusCode.Unauthorized, "Credenciales invalidas");
            return new Response(HttpStatusCode.OK, cl);
        }

        public Response GetAccountsByClient(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            if (cl != null)
            {
                return new Response(HttpStatusCode.OK,_accountRepository
                                                    .FindAccountsByClient(cl.Id)
                                                    .Select(c => new AccountDTO(c)).ToList());
            }
            return new Response(HttpStatusCode.Forbidden, "El cliente no existe en la base de datos");
        }
    }
}
