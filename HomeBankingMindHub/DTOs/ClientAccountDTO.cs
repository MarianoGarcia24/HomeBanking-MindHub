using HomeBankingMindHub.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text.Json.Serialization;

namespace HomeBankingMindHub.DTOs
{
    public class ClientAccountDTO
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ICollection<AccountClientDTO> Accounts { get; set; }
        public ClientAccountDTO(Client client, AccountClientDTO accountClientDTO)
        {
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = new List<AccountClientDTO>();
            Accounts.Add(accountClientDTO);
        }
    }
}
