using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository) 
        {
            _cardRepository = cardRepository;
        }
        public Card GetCardByNumber(string Number)
        {
            Card ca = _cardRepository.FindByNumber(Number);
            if (ca!=null) throw new NullReferenceException();
            return ca;
        }

        public IEnumerable<Card> GetCardsByOwner(long Id)
        {
            IEnumerable<Card> ca = _cardRepository.FindCardsByOwner(Id);
            if (ca==null)
                throw new NullReferenceException();
            return ca;
        }

        public void SaveCard(Card card)
        {
            _cardRepository.Save(card);
        }
    }
}
