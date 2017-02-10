using System;
using DataProvider.Interfaces;

namespace DataProvider.Model
{
    public class CreditTransaction : ITransaction
    {
        public long AccountId { get; set; }
        public long EventId { get; set; }
        public DateTime EventDate { get; set; }
        public string EventDescription { get; set; }
        public double EventAmount { get; set; }
        public double CurrentBalance { get; set; }
        public TransactionType Type { get; set; }
        public string SupplierId { get; set; }
    }
}
