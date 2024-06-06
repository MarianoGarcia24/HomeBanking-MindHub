﻿using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        IEnumerable<Card> GetCardsByOwner(long Id);
        Card GetCardByNumber(string Number);
        void SaveCard(Card card);
        Card CreateCard(NewCardDTO newCard, long clientID, string CardHolder);

    }
}