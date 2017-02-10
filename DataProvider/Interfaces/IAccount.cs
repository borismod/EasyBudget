using System;

namespace DataProvider.Interfaces
{
    public interface IAccount
    {
        VendorId VendorId { get; }
        long AccountId { get; }
        string UserId { get; }
        string Label { get; }
        Double Balance { get; set; }
    }
}
