using System;
using DataProvider.Interfaces;

namespace DataProvider.Model
{
    public class BankAccount : IBankAccount
    {
        public VendorId VendorId { get; set; }
        public long AccountId { get; set; }
        public string UserId { get; set; }
        public string Label { get; set; }
        public string BankName { get; set; }
        public int BankNumber { get; set; }
        public int BranchNumber { get; set; }
        public long AccountNumber { get; set; }
        public Double Balance { get; set; }
    }
}
