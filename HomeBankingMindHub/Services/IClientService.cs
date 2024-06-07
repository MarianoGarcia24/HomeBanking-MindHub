using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        ClientDTO GetClientById(long clientId);
        IEnumerable <ClientDTO> GetAll();
        ClientDTO GetClientByEmail(string email);
        Client CreateClient(ClientSignUpDTO signUpDTO);
        void ValidateCredentials(ClientLoginDTO clientLoginDTO);

    }
}
