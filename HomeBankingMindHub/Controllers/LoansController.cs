using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetLoans()
        {
            try
            {
                Response res = _loanService.GetAllLoans();
                return StatusCode(res.StatusCode, res.Data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        
        public IActionResult PostLoans([FromBody] LoanApplicationDTO NewLoan)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                Response res = email.IsNullOrEmpty() ? new Response(HttpStatusCode.Forbidden, "No hay cliente asociado")
                        : new Response(HttpStatusCode.OK, email);
                if (res.StatusCode == 200) {
                    res = _loanService.CreateNewLoan(NewLoan,email);
                }
                return StatusCode(res.StatusCode,res.Data);
                
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }
    }
}
