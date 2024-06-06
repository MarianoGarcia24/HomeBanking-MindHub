using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        Client GetClientById(long clientId);
        IEnumerable <Client> GetAll();
        Client GetClientByEmail(string email);
        Client CreateClient(ClientSignUpDTO signUpDTO);
        void ValidateCredentials(ClientLoginDTO clientLoginDTO);

    }
}
