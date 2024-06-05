using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardRepository _cardRepository;

        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            try
            {
                var clients = _clientRepository.GetAll();
                var clientsDTO = clients.Select(c => new ClientDTO(c));
                return Ok(clientsDTO);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]

        public IActionResult Get(int id) 
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client != null)
                {
                    var clientDTO = new ClientDTO(client);
                    return Ok(clientDTO);
                }
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                //El cliente guarda la cookie con los datos de su peticion en el navegador
                //Aca preguntamos para que la encueuntre, y si la tiene devuelve el value.
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "No se encontro el usuario logeado");
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "No se encontro el cliente en la base de datos");
                }

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostAccount()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "No se encontro el usuario logeado");
                }
                Client cl = _clientRepository.FindByEmail(email);
                IEnumerable <Account> clAccounts = _accountRepository.FindAccountsByClient(cl.Id);
                if (cl.Accounts.Count == 3)
                {
                    return StatusCode(403, "El cliente no puede tener mas de 3 cuentas");
                }
                Random rnd = new Random();
                int acNumber = rnd.Next(1000, 100000000);
                Account acc = new Account
                {
                    Balance = 0,
                    Number = "VIN - " + acNumber,
                    Client = cl,
                    ClientID = cl.Id,
                    CreationDate = DateTime.Now,
                };
                _accountRepository.Save(acc);
                cl.Accounts.Add(acc);
                _clientRepository.UpdateClient(cl);
                return Created("Cuenta creada con exito", acc);
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult PostCard(NewCardDTO NewCard)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email.IsNullOrEmpty())
                {
                    return StatusCode(403, "No se encontro el usuario logeado");
                }
                if (newAccount.type.IsNullOrEmpty() || newAccount.color.IsNullOrEmpty())
                {
                    return StatusCode(403, "Falta el color, o el tipo de la cuenta");
                }
                Client cl = _clientRepository.FindByEmail(email);
                IEnumerable<Card> clCards = _cardRepository.FindCardsByOwner(cl.Id);
                if (clCards.Count() == 9)
                {
                    return StatusCode(403, "El cliente no puede tener mas de 6 tarjetas");
                }
                if (clCards.Count(card => card.Type == CardType.DEBIT) == 3
                    || clCards.Count(card => card.Type == CardType.CREDIT) == 3)
                {
                    return StatusCode(403, "El cliente ya tiene 3 tarjetas de un tipo");
                }

                string cardNumber = new Random().NextInt64(1000000000000000, 9999999999999999).ToString();

                while (_cardRepository.FindByNumber(cardNumber) != null)
                {
                    cardNumber = new Random().NextInt64(1000000000000000, 9999999999999999).ToString();
                }
                

                Card ca = new Card()
                {
                    ClientId = cl.Id,
                    Client = cl,
                    CardHolder = cl.FirstName + cl.LastName,
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                    Type = (CardType)Enum.Parse(typeof(CardType), NewCard.Type),
                    Color = (ColorType)Enum.Parse(typeof(ColorType), NewCard.Color),
                    CVV = new Random().Next(000, 999),
                    Number = cardNumber
                };

                _cardRepository.Save(ca);
                cl.Cards.Add(ca);
                _clientRepository.UpdateClient(cl);
                return Created("Card created correctly", ca);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] ClientSignUpDTO SignedClient)
        {
            try
            {
                if (String.IsNullOrEmpty(SignedClient.Email) || String.IsNullOrEmpty(SignedClient.Password) ||
                    String.IsNullOrEmpty(SignedClient.FirstName) || String.IsNullOrEmpty(SignedClient.LastName))
                    return StatusCode(403, "Datos Invalidos");

                Client user = _clientRepository.FindByEmail(SignedClient.Email);
                if (user != null) {
                    return StatusCode(403, "El mail está en uso");
                }

                Client cl = new Client
                {
                    Email = SignedClient.Email,
                    Password = SignedClient.Password,
                    FirstName = SignedClient.FirstName,
                    LastName = SignedClient.LastName
                };

                _clientRepository.Save(cl);
                return StatusCode(201,new ClientDTO(SignedClient));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error: " + ex.Message);
            }
        }
    }
}