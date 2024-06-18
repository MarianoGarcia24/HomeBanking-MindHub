using HomeBankingMindHub.DTOs;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface ILoanService
    {
        Response GetAllLoans();
        Response CreateNewLoan(LoanApplicationDTO NewLoan, string email);
    }
}
