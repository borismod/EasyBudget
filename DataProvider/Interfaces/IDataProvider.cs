using System;
using System.Collections.Generic;

namespace DataProvider.Interfaces
{
    public interface IDataProvider : IDisposable
    {
        string Name { get; }
        bool IsReady { get; }
        IEnumerable<IAccount> GetAccounts();
        IEnumerable<ITransaction> GetTransactions(long accountId, DateTime startTime, DateTime endTime);
        double GetBalance(long accountId);
        void RefreshData(long accountId, Func<bool> callback);
    }
}
