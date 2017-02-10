namespace DataProvider.Interfaces
{
  public interface IBankAccount : IAccount
  {
    string BankName { get; }
    int BankNumber { get; }
    int BranchNumber { get; }
    long AccountNumber { get; }
  }
}
