using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOs
{
    public class AccountClientDTO
    {
        public long Id { get; set; }
        public String Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }

        public AccountClientDTO(Account account)
        {
            Id = account.Id;
            Number = account.Number;
            CreationDate = account.CreationDate;
            Balance = account.Balance;
        }
    }
}
