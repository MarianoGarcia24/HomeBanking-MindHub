using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Net;

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

        private Response GetClientEmail() {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            Response res = email.IsNullOrEmpty() ? new Response(HttpStatusCode.Forbidden, "No hay cliente asociado")
                : new Response(HttpStatusCode.OK, email);
            return res;
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
                Response response = _clientService.GetClientById(id);
                return StatusCode(response.StatusCode,response.Data);
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
                
                Response res = _clientService.GetClientByEmail(email);
                return StatusCode(res.StatusCode,res.Data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetAccounts()
        {
            try
            {
                Response res = GetClientEmail();
                if (res.StatusCode == 200)
                {
                    res = _clientService.GetAccountsByClient((string) res.Data);
                }
                return StatusCode(res.StatusCode, res.Data);
            }
            catch (Exception ex) {
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
                Response res = _clientService.GetClientByEmail(email);
                if (res.StatusCode == 200)
                {
                    ClientDTO cl = (ClientDTO)res.Data;
                    AccountClientDTO acc = _accountService.CreateNewAccount(cl.Id);
                    return Created("Cuenta creada con exito", acc);
                }
                return StatusCode(res.StatusCode, res.Data);
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

                Response res = _clientService.GetClientByEmail(email);
                if (res.StatusCode == 200)
                {
                    ClientDTO cl = (ClientDTO)res.Data;
                    CardDTO ca = _cardService.CreateCard(NewCard, cl.Id, cl.FirstName + cl.LastName);
                    return Created("Card created correctly", ca);
                }
                return StatusCode(res.StatusCode, res.Data);

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
                Response res = _clientService.CreateClient(SignedClient);
                if (res.StatusCode == 201) {
                    Client cl = (Client) res.Data;
                    AccountClientDTO ac = _accountService.CreateNewAccount(cl.Id);
                    return StatusCode(201, new ClientAccountDTO(cl, ac));
                }
                return StatusCode(res.StatusCode, res.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error: " + ex.Message);
            }
        }
    }
}