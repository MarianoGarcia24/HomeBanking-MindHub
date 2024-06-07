using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAll()
        {
            try
            {
                Response res = _accountService.GetAllAccounts();
                return StatusCode(res.StatusCode,res.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(int id)
        {
            try
            {
                Response res = _accountService.GetAccountDTOById(id);
                return StatusCode(res.StatusCode, res.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }

        }


    
    }

}
