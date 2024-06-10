﻿using HomeBankingMindHub.DTOs;
using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAccountsByClient(long id);
        Response GetAllAccounts();
        Response GetAccountDTOById(long id);
        AccountClientDTO CreateNewAccount(long clientID);
    }
}
