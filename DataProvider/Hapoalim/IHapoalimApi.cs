using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider.Hapoalim;
using DataProvider.Hapoalim.Responses;

namespace DataProvider.Hapoalim
{
    public interface IHapoalimApi : IDisposable
    {
        bool IsReady { get; }
        string UserId { get; }
        IEnumerable<AccountResponse> GetAccountsData();
        TransactionsResponse GetTransactions(AccountResponse account, DateTime startTime, DateTime endTime);
        BalanceResponse GetBalance(AccountResponse account);
    }
}
