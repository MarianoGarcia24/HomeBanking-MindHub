using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        public AuthController(IClientService clientService, IAccountService accountService) {
            _clientService = clientService;
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO client)
        {
            try
            {
                Response res = _clientService.ValidateCredentials(client);

                if (res.StatusCode != 200)
                    return StatusCode(res.StatusCode, res.Data);

                var claims = new List<Claim>
                {
                    new Claim("Client", client.Email)
                };


                if (client.Email == "kobe23@gmail.com")
                    claims.Add(new Claim("Admin", client.Email));

                //ClaimIdentity ==> se genera una identidad para identificar que alguien esta pidiendo
                // algo. Queremos saber quien esta pidiendo algo, y que es lo que está pidiendo.

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );
                
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)
                    );
                return StatusCode(res.StatusCode, new {IsSuccess="Cliente Autorizado"});

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public  async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
