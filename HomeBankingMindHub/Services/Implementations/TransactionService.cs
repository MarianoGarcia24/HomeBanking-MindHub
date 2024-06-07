using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{
    public class TransactionService
    {
        private readonly AccountRepository _accountRepository;
        private readonly ClientRepository _clientRepository;
        private readonly TransactionRepository _transactionRepository;
        public TransactionService(AccountRepository accountRepository, 
                                  ClientRepository clientRepository, 
                                  TransactionRepository transaction)
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _transactionRepository = transaction;
        }

        private Response ValidateAvailableAmount(string AccountNumber, double Amount)
        {
            Account acc = _accountRepository.FindByAccountNumber(AccountNumber);
            if (acc.Balance - Amount < 0)
            {
                return new Response(HttpStatusCode.Forbidden, "El monto ingresado supera el balance de la cuenta. No se puede tener saldo negativo.");
            }
            return new Response(HttpStatusCode.OK);
        }


        private Response ValidateParameters(NewTransactionDTO NewTransaction)
        {
            if (NewTransaction.Amount == null || NewTransaction.Amount == null
                || NewTransaction.ToAccount == null || NewTransaction.ToAccount == null)
                return new Response(HttpStatusCode.Forbidden, "Hay uno o más parámetros nulos. Intente nuevamente");
            if (String.Equals(NewTransaction.FromAccount, NewTransaction.ToAccount))
                return new Response(HttpStatusCode.Forbidden, "El numero de cuenta destino no puede ser igual al numero de cuenta origen");
            if (_accountRepository.FindByAccountNumber(NewTransaction.ToAccount) == null)
                return new Response(HttpStatusCode.Forbidden, "La cuenta de destino no existe. Ingrese una cuenta destino existente");
            if (_accountRepository.FindByAccountNumber(NewTransaction.FromAccount) == null)
                return new Response(HttpStatusCode.Forbidden, "La cuenta de origen no existe. Ingrese una cuenta origen existente");
            return ValidateAvailableAmount(NewTransaction.FromAccount, NewTransaction.Amount);
            
        }

        private Transaction createTransactionObject(string AccountNumber, double Amount, string Description, string Type)
        {
            return new Transaction
            {
                Amount = Amount,
                Description = Description,
                Date = DateTime.Now,
                Type = (TransactionType)Enum.Parse(typeof(TransactionType), Type),
                AccountId = AccountNumber
            };
        }

        public Response CreateNewTransaction(NewTransactionDTO NewTransaction)
        {
            Response res = ValidateParameters(NewTransaction);
            if (res.StatusCode != 200)
            {
                return res;
            }
            Transaction ToTransaction = new Transaction(NewTransaction.ToAccount, NewTransaction.Amount, NewTransaction.Description,"CREDIT");
            Transaction FromTransaction = new Transaction(NewTransaction.FromAccount, NewTransaction.Amount, NewTransaction.Description,"DEBIT");


        
        }


    }
}
