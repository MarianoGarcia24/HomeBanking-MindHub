using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface ICardService
    {
        IEnumerable<CardDTO> GetCardsByOwner(long Id);
        void SaveCard(Card card);
        Response CreateCard(string email,NewCardDTO newCard);

    }
}
