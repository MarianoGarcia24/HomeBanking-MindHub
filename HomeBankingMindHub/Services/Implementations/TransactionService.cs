using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Implementation;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMindHub.Services.Implementations
{
    public class TransactionService: ITransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository,
                                  IAccountRepository accountRepository, 
                                  IClientRepository clientRepository
                                  )
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _transactionRepository = transactionRepository;
        }

        

        public Response CreateNewTransaction(NewTransactionDTO NewTransaction)
        {
            Response res = ValidateParameters(NewTransaction);
            if (res.StatusCode != 200)
            {
                return res;
            }
            long accountId;

            Transaction FromTransaction = createTransactionObject(NewTransaction.FromAccountNumber, -NewTransaction.Amount,
                                        "Transferencia a cuenta " + NewTransaction.ToAccountNumber + ": "
                                         + NewTransaction.Description,
                                        "DEBIT");

            _transactionRepository.Save(FromTransaction);

            //No hago error handling porque si ocurre algo aca es interno (de la bases de datos)

            Transaction ToTransaction = createTransactionObject(NewTransaction.ToAccountNumber, NewTransaction.Amount,
                                                                "Transferencia desde cuenta " + NewTransaction.FromAccountNumber + ": "
                                                                + NewTransaction.Description, "CREDIT");
                
            _transactionRepository.Save(ToTransaction);

            //Aca lo mismo, las cuentas se que existen por validacion previa, entonces debo continuar sin hacer error handle.
            accountId = _accountRepository.FindByAccountNumber(NewTransaction.ToAccountNumber).Id;

            Account ToAccount = _accountRepository.FindById(accountId);
            ToAccount.Balance += NewTransaction.Amount;
            _accountRepository.Save(ToAccount);

            accountId = _accountRepository.FindByAccountNumber(NewTransaction.FromAccountNumber).Id;

            Account FromAccount = _accountRepository.FindById(accountId);
            FromAccount.Balance -= NewTransaction.Amount;
            _accountRepository.Save(FromAccount);

            return new Response(HttpStatusCode.Created, new TransactionDTO(FromTransaction));
        }

        public Response AccountBelongToUser(string Email, string AccountNumber)
        {
            Client cl = _clientRepository.FindByEmail(Email);
            if (cl == null)
                return new Response(HttpStatusCode.Unauthorized, "El cliente no existe en la base de datos");
            if (cl.Accounts.Count(c => String.Equals(c.Number,AccountNumber)) != 1)
                return new Response(HttpStatusCode.Forbidden, "El numero de cuenta no esta asociado al cliente especificado");
            return new Response(HttpStatusCode.OK);
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
            if (NewTransaction.Amount == 0 || NewTransaction.Description.IsNullOrEmpty()
                || NewTransaction.ToAccountNumber.IsNullOrEmpty() || NewTransaction.ToAccountNumber.IsNullOrEmpty())
                return new Response(HttpStatusCode.Forbidden, "Hay uno o más parámetros nulos. Intente nuevamente");
            if (String.Equals(NewTransaction.FromAccountNumber, NewTransaction.ToAccountNumber))
                return new Response(HttpStatusCode.Forbidden, "El numero de cuenta destino no puede ser igual al numero de cuenta origen");
            if (_accountRepository.FindByAccountNumber(NewTransaction.ToAccountNumber) == null)
                return new Response(HttpStatusCode.Forbidden, "La cuenta de destino no existe. Ingrese una cuenta destino existente");
            if (_accountRepository.FindByAccountNumber(NewTransaction.FromAccountNumber) == null)
                return new Response(HttpStatusCode.Forbidden, "La cuenta de origen no existe. Ingrese una cuenta origen existente");
            return ValidateAvailableAmount(NewTransaction.FromAccountNumber, NewTransaction.Amount);

        }

        private Transaction createTransactionObject(string AccountNumber, double Amount, string Description, string Type)
        {
            long accountId = _accountRepository.FindByAccountNumber(AccountNumber).Id;

            return new Transaction
            {
                Amount = Amount,
                Description = Description,
                Date = DateTime.Now,
                Type = (TransactionType)Enum.Parse(typeof(TransactionType), Type),
                AccountId = accountId,
            };
        }

        private void UpdateAccount(string accountNumber, double Amount)
        {
            Account acc = _accountRepository.FindByAccountNumber(accountNumber);
            acc.Balance += Amount;
            _accountRepository.Save(acc);
        }

    }
}
