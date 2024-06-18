using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOs
{
    public class AccountDTO
    {
        public long Id { get; set; }
        public String Number { get; set; }
        public DateTime CreationDate { get; set; }
        public double Balance { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }

        public AccountDTO(Account account)
        {
            Id = account.Id;
            Number = account.Number;
            CreationDate = account.CreationDate;
            Balance = account.Balance;
            Transactions = account.Transactions.Select(transaction => new TransactionDTO(transaction)).ToList();
        }
        

    }
}