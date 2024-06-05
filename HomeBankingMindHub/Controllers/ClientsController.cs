using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
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
        private readonly IClientService _clientService;
        private readonly ICardService _cardService;
        private readonly IAccountService _accountService;

        public ClientsController(IClientService clientService, ICardService cardService, IAccountService accountService)
        {
            _clientService = clientService;
            _cardService = cardService;
            _accountService = accountService;
        }

        [HttpGet]
/       [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            try
            {
                var clients = _clientService.GetAll();
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
                var client = _clientService.GetClientById(id);
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

                Client client = _clientService.GetClientByEmail(email);
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
                Client cl = _clientService.GetClientByEmail(email);
                if (cl == null)
                {
                    return StatusCode(403, "No se encontro usuario con el mail ingresado");
                }
                var clAccounts = _accountService.GetAccountsByClient(cl.Id);
                if (clAccounts.Count() == 3)
                {
                    return StatusCode(403, "El cliente no puede tener mas de 3 cuentas");
                }
                Random rnd = new Random();
                string acNumber = rnd.Next(1000, 100000000).ToString();
                Account acc = new Account
                {
                    Balance = 0,
                    Number = "VIN - " + acNumber,
                    ClientID = cl.Id,
                    CreationDate = DateTime.Now,
                };
                _accountService.SaveAccount(acc);
                return Created("Cuenta creada con exito", acc);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("current/cards")]
        //[Authorize(Policy = "ClientOnly")]
        //public IActionResult PostCard(NewCardDTO NewCard)
        //{
        //    try
        //    {
        //        string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
        //        if (email.IsNullOrEmpty())
        //        {
        //            return StatusCode(403, "No se encontro el usuario logeado");
        //        }
        //        if (NewCard.Type.IsNullOrEmpty() || NewCard.Color.IsNullOrEmpty())
        //        {
        //            return StatusCode(403, "Falta el color, o el tipo de la cuenta");
        //        }
        //        Client cl = _clientRepository.FindByEmail(email);
        //        var clCards = _cardRepository.FindCardsByOwner(cl.Id);
        //        if (clCards.Count() == 6)
        //        {
        //            return StatusCode(403, "El cliente no puede tener mas de 6 tarjetas");
        //        }

        //        var newCardType = (CardType)Enum.Parse(typeof(CardType), NewCard.Type);

        //        if (clCards.Count(card => card.Type == newCardType) == 3)
        //        {
        //            return StatusCode(403, "El cliente no puede tener mas de 3 tarjetas de " + NewCard.Type);
        //        }

        //        string cardNumber = new Random().NextInt64(1000000000000000, 9999999999999999).ToString();

        //        while (_cardRepository.FindByNumber(cardNumber) != null)
        //        {
        //            cardNumber = new Random().NextInt64(1000000000000000, 9999999999999999).ToString();
        //        }


        //        Card ca = new Card()
        //        {
        //            ClientId = cl.Id,
        //            CardHolder = cl.FirstName + cl.LastName,
        //            FromDate = DateTime.Now,
        //            ThruDate = DateTime.Now.AddYears(5),
        //            Type = newCardType,
        //            Color = (ColorType) Enum.Parse(typeof(ColorType), NewCard.Color),
        //            CVV = new Random().Next(000, 999),
        //            Number = cardNumber
        //        };

        //        _cardRepository.Save(ca);
        //        return Created("Card created correctly", ca);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        //[HttpPost]
        //public IActionResult Post([FromBody] ClientSignUpDTO SignedClient)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(SignedClient.Email) || String.IsNullOrEmpty(SignedClient.Password) ||
        //            String.IsNullOrEmpty(SignedClient.FirstName) || String.IsNullOrEmpty(SignedClient.LastName))
        //            return StatusCode(403, "Datos Invalidos");

        //        Client user = _clientRepository.FindByEmail(SignedClient.Email);
        //        if (user != null) {
        //            return StatusCode(403, "El mail está en uso");
        //        }

        //        Client cl = new Client
        //        {
        //            Email = SignedClient.Email,
        //            Password = SignedClient.Password,
        //            FirstName = SignedClient.FirstName,
        //            LastName = SignedClient.LastName
        //        };

        //        _clientRepository.Save(cl);
        //        return StatusCode(201,new ClientDTO(SignedClient));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Server Error: " + ex.Message);
        //    }
        //}
    }
}