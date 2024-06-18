using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{

    public class LoanService : ILoanService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;
        public LoanService(ILoanRepository loanRepository,
                           IAccountRepository accountRepository,
                           IClientRepository clientRepository,
                           IClientLoanRepository clientLoanRepository,
                           ITransactionRepository transactionRepository
                           )
        {
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
            _clientLoanRepository = clientLoanRepository;
        }

        private Response ValidateProperties(LoanApplicationDTO NewLoan, string email)
        {

            Loan loan = _loanRepository.FindById(NewLoan.LoanId);
            Account account = _accountRepository.FindByAccountNumber(NewLoan.ToAccountNumber);
            Client client = _clientRepository.FindByEmail(email);

            if (NewLoan.Payments.IsNullOrEmpty() || NewLoan.Amount <= 0)
                return new Response(HttpStatusCode.Forbidden, "El monto o las cuotas son invalidos");
            if (loan == null)
                return new Response(HttpStatusCode.Forbidden, "El prestamo ingresado no existe");
            if (loan.MaxAmount < NewLoan.Amount)
                return new Response(HttpStatusCode.Forbidden, "El monto supera el maximo permitido para el tipo de prestamo");
            if (!loan.Payments.Contains(NewLoan.Payments))
                return new Response(HttpStatusCode.Forbidden, "El numero de cuotas es invalido");
            if (account == null)
                return new Response(HttpStatusCode.Forbidden, "La cuenta ingresada no existe");

            if (account.ClientID != client.Id)
                return new Response(HttpStatusCode.Forbidden, "La cuenta destino no pertenece al cliente autenticado");

            return new Response(HttpStatusCode.OK);
        }

        public Response CreateNewLoan(LoanApplicationDTO NewLoan, string email)
        {
            using var dbContextTransaction = _loanRepository.BeginTransaction();
                try
                {
                Response res = ValidateProperties(NewLoan, email);

                if (res.StatusCode == 200)
                {
                    {
                        Account acc = _accountRepository.FindByAccountNumber(NewLoan.ToAccountNumber);
                        Client cl = _clientRepository.FindByEmail(email);
                        Loan loan = _loanRepository.FindById(NewLoan.LoanId);

                        Transaction tr = new()
                        {
                            Amount = NewLoan.Amount,
                            AccountId = acc.Id,
                            Date = DateTime.Now,
                            Description = _loanRepository.FindById(NewLoan.LoanId).Name + ": loan aprroved",
                            Type = Models.utils.TransactionType.CREDIT,
                        };

                        _transactionRepository.Save(tr);

                        acc.Balance += NewLoan.Amount;
                        _accountRepository.Save(acc);

                        ClientLoan cloan = new()
                        {
                            Amount = NewLoan.Amount * 1.20,
                            ClientId = cl.Id,
                            LoanId = NewLoan.LoanId,
                            Payments = NewLoan.Payments,
                        };

                        _clientLoanRepository.Save(cloan);

                        res = new Response(HttpStatusCode.OK, new ClientLoanDTO()
                        {
                            Amount = cloan.Amount,
                            LoanId = NewLoan.LoanId,
                            Name = loan.Name,
                            Payments = int.Parse(NewLoan.Payments)

                        });

                        dbContextTransaction.Commit();
                    }
                  
                }

                return res;
            }
            catch (Exception ex)
            {
                dbContextTransaction.Rollback();
                return new Response(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public Response GetAllLoans()
        {
            return new Response(HttpStatusCode.OK,_loanRepository.GetAll());
        }

        
    }
}
