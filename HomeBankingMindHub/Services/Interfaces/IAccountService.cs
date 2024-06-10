using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IAccountService
    {
        Response GetAllAccounts();
        Response GetAccountDTOById(long id);
    }
}
