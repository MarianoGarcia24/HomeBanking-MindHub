using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IClientService
    {
        Response GetClientById(long clientId);
        IEnumerable<ClientDTO> GetAll();
        Response GetClientByEmail(string email);
        Response CreateClient(ClientSignUpDTO signUpDTO);
        Response ValidateCredentials(ClientLoginDTO clientLoginDTO);

    }
}
