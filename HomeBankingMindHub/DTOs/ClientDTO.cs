using HomeBankingMindHub.Models;
using System.Text.Json.Serialization;

namespace HomeBankingMindHub.DTOs
{
    public class ClientDTO
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<AccountClientDTO> Accounts { get; set; }
        public ICollection<ClientLoanDTO> Loans { get; set; }
        public ICollection<CardDTO> Cards { get; set; }
        public ClientDTO(Client client)
        {
            Id = client.Id;
            Email = client.Email;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Accounts = client.Accounts.Select(account => new AccountClientDTO(account)).ToList();
            Loans = client.ClientLoans.Select(cl => new ClientLoanDTO(cl)).ToList();
            Cards = client.Cards.Select(ca => new CardDTO(ca)).ToList();
        }

        public ClientDTO(ClientSignUpDTO client)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;           
        }
    }
}
