using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
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
        public IActionResult Get(int id) {
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
                    return StatusCode(403,"No se encontro el usuario logeado");
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