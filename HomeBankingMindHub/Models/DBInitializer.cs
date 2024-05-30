using HomeBankingMindHub.Models.utils;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client{ FirstName="Kobe", LastName="Bryant", Email= "kobe23@gmail.com", Password = "123" },
                    new Client{ FirstName="Michael B", LastName="Jordan", Email= "LebronJames@gmail.com", Password="123" },
                    new Client{ FirstName="Juan Roman", LastName="Riquelme", Email= "jrr10@gmail.com", Password = "123" },
                    new Client{ FirstName="Martin", LastName="Palermo", Email= "mp9@gmail.com", Password="123" },
                    new Client{ FirstName="Sebastian", LastName="Battaglia", Email= "labattaglioneta@gmail.com", Password="123" },
                    new Client{ FirstName="Marianito", LastName="El Garcia", Email= "mariangarcia@gmail.com", Password="123" },
                    new Client{ FirstName="D O", LastName="G G", Email= "dodoublegg@gmail.com", Password="123" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
                
            }

            if (!context.Accounts.Any())
            {
                var client_account = context.Clients.FirstOrDefault(c => c.Email == "jrr10@gmail.com");
                var accounts = new Account[]
                {
                    new Account{ ClientID = 1, CreationDate = DateTime.Now, Number= "VIN0978", Balance = 250000},
                    new Account{ ClientID = 3, CreationDate = DateTime.Now, Number= "VIN0032", Balance = 1000000},
                    new Account{ ClientID = 2, CreationDate = DateTime.Now, Number= "VIN0082", Balance = 24570},
                    new Account{ ClientID = 7, CreationDate = DateTime.Now, Number= "VIN4444", Balance = 0},
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "VIN0001", Balance = 1550},
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "USD0001", Balance = 1890},
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "VIN0002", Balance = 360000}
                };
                foreach (Account account in accounts) {
                    context.Accounts.Add(account);
                }
                context.SaveChanges();
            }

            if (!context.Transactions.Any())
            {
                var transactions = new Transaction[]
                {
                    new() { AccountId = 1, Amount = 1041.67, Description = "Camara web Mercado Libre",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT.ToString()},
                    new() { AccountId = 2, Amount = 2390, Description = "Coca Cola en botella de vidrio",  Date = DateTime.Now.AddDays(-5), Type = TransactionType.DEBIT.ToString()},
                    new() { AccountId = 2, Amount = 19070, Description = "Coca Cola añejada en botella residual",  Date = DateTime.Now.AddHours(10), Type = TransactionType.DEBIT.ToString()},
                    new() { AccountId = 3, Amount = 27600, Description = "Motor de auto",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT.ToString()},
                    new() { AccountId = 7, Amount = 27600, Description = "Cocarda",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT.ToString()}
                };
                foreach (Transaction transaction in transactions)
                {
                    context.Transactions.Add(transaction);
                }
                context.SaveChanges();
            }





        }
    }
}
