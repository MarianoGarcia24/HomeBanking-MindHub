﻿using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetCurrent()
        {
            try
            {
                //El cliente guarda la cookie con los datos de su peticion en el navegador
                //Aca preguntamos para que la encueuntre, y si la tiene devuelve el value.
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO(client);
                return Ok(clientDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}