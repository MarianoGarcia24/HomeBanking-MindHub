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
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_clientService.GetAll());
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
                ClientDTO client = _clientService.GetClientById(id);
                return Ok(client);
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
                
                ClientDTO client = _clientService.GetClientByEmail(email);
                if (client == null)
                {
                    return StatusCode(403, "No se encontro el cliente en la base de datos");
                }

                return Ok(client);

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

                ClientDTO cl = _clientService.GetClientByEmail(email);
                AccountClientDTO acc = _accountService.CreateNewAccount(cl.Id);
                return Created("Cuenta creada con exito", acc);
            }
            catch(InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("current/cards")]
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

                ClientDTO cl = _clientService.GetClientByEmail(email);
                CardDTO ca = _cardService.CreateCard(NewCard, cl.Id, cl.FirstName + cl.LastName);
                return Created("Card created correctly", ca);
            }
            catch(NullReferenceException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public IActionResult Post([FromBody] ClientSignUpDTO SignedClient)
        {
            try
            {
                Client cl = _clientService.CreateClient(SignedClient);
                AccountClientDTO ac = _accountService.CreateNewAccount(cl.Id);
                return StatusCode(201, new ClientAccountDTO(cl,ac));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error: " + ex.Message);
            }
        }
    }
}