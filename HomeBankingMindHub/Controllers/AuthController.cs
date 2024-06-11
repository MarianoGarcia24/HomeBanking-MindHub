using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Interfaces;
using HomeBankingMindHub.utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAccountService _accountService;
        private readonly Utilities _utilities;
        public AuthController(IClientService clientService, IAccountService accountService, Utilities utilities) {
            _clientService = clientService;
            _accountService = accountService;
            _utilities = utilities;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO client)
        {
            try
            {
                Response res = _clientService.ValidateCredentials(client);

                if (res.StatusCode != 200)
                    return StatusCode(res.StatusCode, res.Data);


                //ClaimIdentity ==> se genera una identidad para identificar que alguien esta pidiendo
                // algo. Queremos saber quien esta pidiendo algo, y que es lo que está pidiendo.


                return StatusCode(res.StatusCode, new { isSuccess = true, token = _utilities.generateJWT((Client)res.Data) });

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
