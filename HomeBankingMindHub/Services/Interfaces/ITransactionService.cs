using HomeBankingMindHub.DTOs;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface ITransactionService
    {
        Response AccountBelongToUser(string email, string AccountNumber);
        Response CreateNewTransaction(NewTransactionDTO NewTransaction);
    }
}
