using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.utils;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace HomeBankingMindHub.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IClientRepository _clientRepository;

        public CardService(ICardRepository cardRepository, IClientRepository clientRepository)
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
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
            Random random = new Random();
            do
            {
                cardNumber = new Random().Next(1000, 10000).ToString();
                for (var i = 0; i < 3; i++)
                {
                    cardNumber = cardNumber + " " + random.Next(1000, 10000).ToString();
                }
            } while (_cardRepository.FindByNumber(cardNumber) != null);
            return cardNumber;
        }


        public Response CreateCard(string email, NewCardDTO NewCard)
        {
            if (!email.IsNullOrEmpty())
            {
                Client cl = _clientRepository.FindByEmail(email);
                if (cl != null)
                {
                    if (NewCard.Type.IsNullOrEmpty() || NewCard.Color.IsNullOrEmpty())
                    {
                        return new Response(HttpStatusCode.Forbidden, "La tarjeta no posee color o tipo");
                    }

                    IEnumerable<Card> cards = _cardRepository.FindCardsByOwner(cl.Id);

                    if (cards.Count() == 6)
                    {
                        return new Response(HttpStatusCode.Forbidden, "El cliente no puede tener mas de 6 tarjetas");
                    }

                    CardType newCardType = (CardType)Enum.Parse(typeof(CardType), NewCard.Type);

                    if (cards.Count(c => c.Type == newCardType) > 2)
                        return new Response(HttpStatusCode.Forbidden, "El cliente ya tiene 3 tarjetas del mismo tipo");

                    string cardNumber = generateNewCardNumber();

                    Card ca = new Card()
                    {
                        ClientId = cl.Id,
                        CardHolder = cl.FirstName + " " + cl.LastName,
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5),
                        Type = newCardType,
                        Color = (ColorType)Enum.Parse(typeof(ColorType), NewCard.Color),
                        CVV = new Random().Next(000, 999),
                        Number = cardNumber
                    };

                    _cardRepository.Save(ca);
                    return new Response(HttpStatusCode.Created, new CardDTO(_cardRepository.FindByNumber(cardNumber)));

                }
            }
            return new Response(HttpStatusCode.Forbidden, "El mail es invalido");
        }

    }
}
