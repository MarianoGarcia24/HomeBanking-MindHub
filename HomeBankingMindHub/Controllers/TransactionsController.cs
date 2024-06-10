using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Implementations;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        [Authorize (Policy = "ClientOnly")]
        public IActionResult Post([FromBody] NewTransactionDTO NewTransaction)
        {
            try
            {
                Response res;
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : null;
                if (email.IsNullOrEmpty())
                    return BadRequest("No hay usuario logeado");

                res = _transactionService.AccountBelongToUser(email, NewTransaction.FromAccountNumber);
                if (res.StatusCode == 200)
                    res = _transactionService.CreateNewTransaction(NewTransaction);
                return StatusCode(res.StatusCode, res.Data);
            }
            catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
           

            
        }
    }
}
