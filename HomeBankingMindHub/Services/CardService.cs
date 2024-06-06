using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;

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

        public Card CreateCard(NewCardDTO NewCard, long clientID, string CardHolder)
        {
            if (NewCard.Type.IsNullOrEmpty() || NewCard.Color.IsNullOrEmpty())
            {
                throw new NullReferenceException("La tarjeta no posee color o tipo");
            }
            if (_cardRepository.FindCardsByOwner(clientID).Count() == 6)
            {
                throw new Exception("El cliente tiene el maximo de tarjetas alcanzadas, no puede crear nuevas.");
            }

            string cardNumber = generateNewCardNumber();
            var newCardType = (CardType)Enum.Parse(typeof(CardType), NewCard.Type);
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
            return ca;
        }


    }
}
