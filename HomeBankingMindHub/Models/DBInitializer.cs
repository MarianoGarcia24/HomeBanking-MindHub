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
                    new Client{ FirstName="Kobe", LastName="Bryant", email= "kobe23@gmail.com", password="123" },
                    new Client{ FirstName="Michael B", LastName="Jordan", email= "LebronJames@gmail.com", password="123" },
                    new Client{ FirstName="Juan Roman", LastName="Riquelme", email= "jrr10@gmail.com", password="123" },
                    new Client{ FirstName="Martin", LastName="Palermo", email= "mp9@gmail.com", password="123" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }

        }
    }
}
