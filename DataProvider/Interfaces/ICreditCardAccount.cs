
namespace DataProvider.Interfaces
{
  public interface ICreditCardAccount : IAccount
  {
    string CompanyName { get; set; }
    long CardNumber { get; set; }
    string CardName { get; set; }
    int PaymentDate { get; set; }
    int BankId { get; set; }
    int BankBranchId { get; set; }
    long BankAccountId { get; set; }
    string PartnerName { get; set; }
  }
}
