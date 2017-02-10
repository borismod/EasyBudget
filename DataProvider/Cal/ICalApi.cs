using System;
using System.Collections.Generic;
using DataProvider.Interfaces;

namespace DataProvider.Cal
{
    public interface ICalApi : IDisposable
    {
        bool IsReady { get; }
        IList<ICreditCardAccount> GetCards();
        IList<ITransaction> GetTransactions(long cardIndex, int month, int year);
    }
}
