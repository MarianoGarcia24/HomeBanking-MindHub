using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        IEnumerable<CardDTO> GetCardsByOwner(long Id);
        CardDTO GetCardByNumber(string Number);
        void SaveCard(Card card);
        CardDTO CreateCard(NewCardDTO newCard, long clientID, string CardHolder);

    }
}
