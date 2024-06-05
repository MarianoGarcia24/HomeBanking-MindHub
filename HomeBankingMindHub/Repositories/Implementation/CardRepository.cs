using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Repositories.Implementation
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Card> GetAll()
        {
            return FindAll()
                .ToList();
        }

        public Card FindById(long id)
        {
            return FindByCondition(ca => ca.Id == id)
                .FirstOrDefault();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }

        public IEnumerable<Card> FindCardsByOwner(long ownerId)
        {
            return FindByCondition(ca => ca.ClientId == ownerId)
                .ToList();
        }

        public Card FindByNumber(string number)
        {
            return FindByCondition(ca => ca.Number == number)
                .FirstOrDefault();
        }
    }
}
