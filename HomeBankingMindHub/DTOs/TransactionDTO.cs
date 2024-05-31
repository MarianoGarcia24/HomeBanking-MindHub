using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOs
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public TransactionDTO(Transaction transaction)
        {
            Id = transaction.Id;
            Amount = transaction.Amount;
            Description = transaction.Description;
            Date = transaction.Date;
            Console.WriteLine(transaction.Type);
            if ((int) transaction.Type == 1)
            {
                Type = "CREDIT";
            }
            else
            {
                Type = "DEBIT";
            }
        }
    }
}
