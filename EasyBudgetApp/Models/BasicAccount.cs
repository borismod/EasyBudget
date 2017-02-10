
using DataProvider;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Models
{
    public class BasicAccount : IAccount
    {
        public VendorId VendorId { get; }
        public long AccountId { get; }
        public string UserId { get; }
        public string Label { get; }
        public double Balance { get; set; }
        public bool IsEnabled { get; set; }

        public BasicAccount(string name, long id, bool isEnabled, string userId)
        {
            VendorId = VendorId.None;
            AccountId = id;
            Label = name;
            UserId = userId;
            IsEnabled = isEnabled;
            Balance = double.NaN;
        }
    }
}
