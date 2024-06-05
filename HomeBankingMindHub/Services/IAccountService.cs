﻿using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        IEnumerable <Account> GetAccountsByClient(long id);
        void SaveAccount(Account account);
        IEnumerable <Account> GetAllAccounts();
        Account GetAccountById(long id);
    }
}
