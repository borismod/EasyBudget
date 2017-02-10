using System;

namespace DataProvider.Interfaces
{
    public enum TransactionType { Income, Expense };

    public interface ITransaction
    {
        long AccountId { get; set; }
        long EventId { get; set; }
        DateTime EventDate { get; }
        string EventDescription { get; }
        Double EventAmount { get; set; }
        Double CurrentBalance { get; set; }
        TransactionType Type { get; set; }
        string SupplierId { get; set; }
    }
}
