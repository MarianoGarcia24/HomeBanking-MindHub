using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        Card FindById(long id);
        Card FindByNumber(string number);
        IEnumerable<Card> FindCardsByOwner(long id);
        IEnumerable<Card> GetAll();
        void Save(Card card);

    }
}
