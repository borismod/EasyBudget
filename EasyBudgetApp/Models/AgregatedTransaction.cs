using System;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Models
{
    public class AgregatedTransaction : ITransaction
    {
        private static long _idCounter;

        public long AccountId { get; set; }
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public double EventAmount { get; set; }
        public double CurrentBalance { get; set; }
        public TransactionType Type { get; set; }
        public string SupplierId { get; set; }

        public AgregatedTransaction()
        {
            EventId = ++_idCounter;
        }
    }
}
