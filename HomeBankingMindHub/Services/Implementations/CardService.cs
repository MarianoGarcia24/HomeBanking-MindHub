using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;

        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }
        public CardDTO GetCardByNumber(string Number)
        {
            Card ca = _cardRepository.FindByNumber(Number);
            if (ca != null) throw new NullReferenceException();
            return new CardDTO(ca);
        }

        public IEnumerable<CardDTO> GetCardsByOwner(long Id)
        {
            IEnumerable<CardDTO> ca = _cardRepository.FindCardsByOwner(Id).Select(c => new CardDTO(c)).ToList();
            if (ca == null)
                throw new NullReferenceException();
            return ca;
        }

        public void SaveCard(Card card)
        {
            _cardRepository.Save(card);
        }


        private string generateNewCardNumber()
        {
            string cardNumber;
            do
            {
                cardNumber = new Random().NextInt64(1000000000000000, 9999999999999999).ToString();
                cardNumber = System.Text.RegularExpressions.Regex.Replace(cardNumber, ".{4}", "$0");
            } while (_cardRepository.FindByNumber(cardNumber) != null);
            return cardNumber;
        }


        public CardDTO CreateCard(NewCardDTO NewCard, long clientID, string CardHolder)
        {
            if (NewCard.Type.IsNullOrEmpty() || NewCard.Color.IsNullOrEmpty())
            {
                throw new NullReferenceException("La tarjeta no posee color o tipo");
            }
            IEnumerable<Card> cards = _cardRepository.FindCardsByOwner(clientID);
            if (cards.Count() == 6)
            {
                throw new Exception("El cliente tiene el maximo de tarjetas alcanzadas, no puede crear nuevas.");
            }

            CardType newCardType = (CardType)Enum.Parse(typeof(CardType), NewCard.Type);

            if (cards.Count(c => c.Type == newCardType) > 2)
                throw new Exception("El cliente ya tiene 3 tarjetas del imsmo tipo");

            string cardNumber = generateNewCardNumber();
            Card ca = new Card()
            {
                ClientId = clientID,
                CardHolder = CardHolder,
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                Type = newCardType,
                Color = (ColorType)Enum.Parse(typeof(ColorType), NewCard.Color),
                CVV = new Random().Next(000, 999),
                Number = cardNumber
            };
            SaveCard(ca);
            CardDTO AuxCard = new CardDTO(_cardRepository.FindByNumber(cardNumber));
            return AuxCard;
        }


    }
}
