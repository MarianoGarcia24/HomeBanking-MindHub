using HomeBankingMindHub.Models.utils;

namespace HomeBankingMindHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            AddClients(context);
            AddAccounts(context);
            AddTransactions(context);
            AddLoans(context);
            AddCards(context);
        }

        private static void AddLoans(HomeBankingContext context)
        {
            if (!context.Loans.Any())
            {
                var Loans = new Loan[]
                {
                    new() { MaxAmount = 800000, Name="Hipotecario", Payments="36"},
                    new() { MaxAmount = 10000, Name="Personal", Payments="12,18,24,26,30"},
                    new() { MaxAmount = 300000, Name="PyMe", Payments="12,24,36,48"},
                    new() { MaxAmount = 15000, Name="Automotriz", Payments="6,12,24,36"}
                };

                foreach (Loan loan in Loans)
                {
                    context.Loans.Add(loan);
                }
                context.SaveChanges();

                var client1 = context.Clients.FirstOrDefault(c => c.Email == "kobe23@gmail.com");
                if (client1 != null) {

                    var loan = context.Loans.FirstOrDefault(loan => loan.Name == "Hipotecario");
                    if (loan != null)
                    {
                        var clientLoan1 = new ClientLoan { Amount = 500000, ClientId = client1.Id, Client = client1, Loan = loan, LoanId = loan.Id, Payments = "36" };
                        context.ClientLoans.Add(clientLoan1);
                    }
                }
                var loan1 = context.Loans.FirstOrDefault(l => l.Name == "PyMe");
                if (loan1!= null)
                {
                    var clientLoan2 = new ClientLoan { Amount = 278000, ClientId = client1.Id, Client = client1, Loan = loan1, LoanId = loan1.Id, Payments = "24" };
                    context.ClientLoans.Add(clientLoan2);
                }
                var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                if (loan2 != null)
                {
                    var clientLoan3 = new ClientLoan { Amount = 10000, ClientId = client1.Id, Client = client1, Loan = loan2, LoanId = loan2.Id, Payments = "12" };
                    context.ClientLoans.Add(clientLoan3);
                }
                context.SaveChanges(); 
            }
        }

        private static void AddClients(HomeBankingContext context)
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
        }

        private static void AddAccounts(HomeBankingContext context)
        {
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
                foreach (Account account in accounts)
                {
                    context.Accounts.Add(account);
                }
                context.SaveChanges();
            }
        }

        private static void AddTransactions(HomeBankingContext context)
        {
            if (!context.Transactions.Any())
            {
                var transactions = new Transaction[]
                {
                    new() { AccountId = 1, Amount = 1041.67, Description = "Camara web Mercado Libre",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT},
                    new() { AccountId = 2, Amount = 2390, Description = "Coca Cola en botella de vidrio",  Date = DateTime.Now.AddDays(-5), Type = TransactionType.DEBIT},
                    new() { AccountId = 2, Amount = 19070, Description = "Coca Cola añejada en botella residual",  Date = DateTime.Now.AddHours(10), Type = TransactionType.DEBIT},
                    new() { AccountId = 3, Amount = 27600, Description = "Motor de auto",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT},
                    new() { AccountId = 7, Amount = 27600, Description = "Cocarda",  Date = DateTime.Now.AddMonths(2), Type = TransactionType.CREDIT}
                };
                foreach (Transaction transaction in transactions)
                {
                    context.Transactions.Add(transaction);
                }
                context.SaveChanges();
            }
        }

        private static void AddCards(HomeBankingContext context)
        {
            if (!context.Cards.Any())
            {
                var client = context.Clients.FirstOrDefault(cl => cl.Id == 1);
                var cards = new Card[]
                {
                    new() { ClientId = client.Id, CardHolder=client.FirstName + " " + client.LastName, Color = ColorType.TITANIUM, Type = CardType.CREDIT, CVV = 999, FromDate=DateTime.Now.AddDays(-30), ThruDate=DateTime.Now.AddYears(1), Number="4444 9899 3752 6264"},
                    new() { ClientId = client.Id, CardHolder=client.FirstName + " " + client.LastName, Color = ColorType.GOLD, Type = CardType.DEBIT, CVV = 982, FromDate=DateTime.Now, ThruDate=DateTime.Now.AddYears(10), Number="4589 2948 3409 2670"},
                    new() { ClientId = client.Id, CardHolder=client.FirstName + " " + client.LastName, Color = ColorType.SILVER, Type = CardType.CREDIT, CVV = 230, FromDate=DateTime.Now, ThruDate=DateTime.Now.AddMonths(5), Number="4523 8927 8346 2109"}
                }; 

                foreach(Card card in cards)
                {
                    context.Cards.Add(card);
                }
                context.SaveChanges();
            }
        }
    }


}
