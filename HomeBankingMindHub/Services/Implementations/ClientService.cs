using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
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

        private string GenerateNewAccountNumber()
        {
            string acNumber;
            do
            {
                acNumber = new Random().Next(1000, 100000000).ToString();
            } while (_accountRepository.FindByAccountNumber(acNumber) != null);
            return acNumber;
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
            string AccountNumber = GenerateNewAccountNumber();

            Account acc = new Account
            {
                Balance = 0,
                ClientID = cl.Id,
                Number = "VIN" + AccountNumber,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);

            cl = FindClientByEmail(cl.Email);
            AccountClientDTO account = new AccountClientDTO(_accountRepository.FindByAccountNumber(acc.Number));
            return new Response(HttpStatusCode.Created, new ClientAccountDTO(cl,account));
        
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

        public Response CreateNewAccount(string email)
        {
            Client cl = _clientRepository.FindByEmail(email);
            var clAccounts = _accountRepository.FindAccountsByClient(cl.Id);
            if (clAccounts.Count() == 3)
            {
                throw new InvalidOperationException("Numero de cuentas máximo alcanzado. El cliente posee 3 cuentas.");
            }

            string acNumber = GenerateNewAccountNumber();
            Account acc = new Account
            {
                Balance = 0,
                Number = "VIN" + acNumber,
                ClientID = cl.Id,
                CreationDate = DateTime.Now,
            };
            _accountRepository.Save(acc);
            Account acc2 = _accountRepository.FindByAccountNumber(acc.Number);
            return new Response(HttpStatusCode.Created,new AccountDTO(acc2));
        }

        public Response CreateNewCard(string email, NewCardDTO NewCard)
        {
            if (!email.IsNullOrEmpty())
            {
                Client cl = FindClientByEmail(email);
                if (cl != null)
                {
                    if (NewCard.Type.IsNullOrEmpty() || NewCard.Color.IsNullOrEmpty())
                    {
                        return new Response(HttpStatusCode.Forbidden,"La tarjeta no posee color o tipo");
                    }
                    IEnumerable<Card> cards = _cardRepository.FindCardsByOwner(cl.Id);

                    if (cards.Count() == 6)
                    {
                        return new Response(HttpStatusCode.Forbidden, "El cliente no puede tener mas de 6 tarjetas");
                    }

                    CardType newCardType = (CardType)Enum.Parse(typeof(CardType), NewCard.Type);
                    if (cards.Count(c => c.Type == newCardType) > 2)
                        return new Response(HttpStatusCode.Forbidden,"El cliente ya tiene 3 tarjetas del mismo tipo");

                    string cardNumber = generateNewCardNumber();
                    Card ca = new Card()
                    {
                        ClientId = cl.Id,
                        CardHolder = cl.FirstName + " " + cl.LastName,
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5),
                        Type = newCardType,
                        Color = (ColorType)Enum.Parse(typeof(ColorType), NewCard.Color),
                        CVV = new Random().Next(000, 999),
                        Number = cardNumber
                    };
                    _cardRepository.Save(ca);
                    CardDTO AuxCard = new CardDTO(_cardRepository.FindByNumber(cardNumber));
                    return new Response(HttpStatusCode.Created,AuxCard);

                }
            }
            return new Response(HttpStatusCode.Forbidden,"El mail es invalido");
        }

        private string generateNewCardNumber()
        {
            string cardNumber;
            Random random = new Random();
            do
            {
                cardNumber = new Random().Next(1000, 10000).ToString();
                for (var i = 0; i < 3; i++)
                {
                    cardNumber = cardNumber + " " + random.Next(1000, 10000).ToString();
                }
            } while (_cardRepository.FindByNumber(cardNumber) != null);
            return cardNumber;
        }
    }
}
