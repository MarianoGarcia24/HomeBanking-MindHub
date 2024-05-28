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
                    new Client{ FirstName="Sebastian", LastName="Battaglia", Email= "labattaglioneta@gmail.com", Password="123" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
                
            }

            if (!context.Accounts.Any())
            {
                var client_account = context.Clients.FirstOrDefault(c => c.Email == "jrr10@gmail.com");
                var accounts = new Account[]
                {
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "VIN0001", Balance = 0},
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "USD0001", Balance = 0},
                    new Account{ ClientID = client_account.Id, CreationDate = DateTime.Now, Number= "VIN0002", Balance = 0}
                };
                foreach (Account account in accounts) {
                    context.Accounts.Add(account);
                }
                context.SaveChanges();
            }
            

        }
    }
}
