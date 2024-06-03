using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        Card FindById(long id);
        IEnumerable<Card> GetAll();
        void Save(Card card);
    }
}
