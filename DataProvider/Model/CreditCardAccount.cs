using DataProvider.Interfaces;

namespace DataProvider.Model
{
    public class CreditCardAccount : ICreditCardAccount
    {
        public VendorId VendorId { get; set; }
        public long AccountId { get; set; }
        public string UserId { get; set; }
        public string CompanyName { get; set; }
        public string Label { get; set; }
        public double Balance { get; set; }
        public long CardNumber { get; set; }
        public string CardName { get; set; }
        public int PaymentDate { get; set; }
        public int BankId { get; set; }
        public int BankBranchId { get; set; }
        public long BankAccountId { get; set; }
        public string PartnerName { get; set; }
    }
}
