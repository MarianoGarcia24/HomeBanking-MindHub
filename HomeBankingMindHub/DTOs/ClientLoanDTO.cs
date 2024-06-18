using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.DTOs
{
    public class ClientLoanDTO
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int Payments { get; set; }
        public double Amount { get; set; }
        public long LoanId { get; set; }
        public ClientLoanDTO(ClientLoan cl)
        {
            ID = cl.Id;
            Name = cl.Loan.Name;
            Payments = int.Parse(cl.Payments);
            Amount = cl.Amount;
            LoanId = cl.LoanId; 
        }

        public ClientLoanDTO()
        {
         
        }

    }
}
