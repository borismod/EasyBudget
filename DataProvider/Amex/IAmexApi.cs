using System;
using System.Collections.Generic;
using DataProvider.Amex.Responses;

namespace DataProvider.Amex
{
    public interface IAmexApi : IDisposable
    {
        bool IsReady { get; }
        string UserId { get; }
        CardListDeatils GetCards();
        IEnumerable<CardTransaction> GetTransactions(long cardIndex, int month, int year);
    }
}
