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
        double EventAmount { get; set; }
        double CurrentBalance { get; set; }
        TransactionType Type { get; set; }
        string SupplierId { get; set; }
    }
}
