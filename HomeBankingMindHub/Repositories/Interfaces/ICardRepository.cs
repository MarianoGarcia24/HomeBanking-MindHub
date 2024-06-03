using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        Card GetById(long id);
        IEnumerable<Card> GetAll();
        void Save(Card card);
    }
}
