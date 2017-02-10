namespace DataProvider.Hapoalim.Responses
{
  public class AccountResponse
  {
    public int BankNumber { get; set; }
    public int ExtendedBankNumber { get; set; }
    public int BranchNumber { get; set; }
    public long AccountNumber { get; set; }
    public int PartyPreferredIndication { get; set; }
    public int PartyAccountInvolvementCode { get; set; }
    public string PartyAccountInvolvementDescription { get; set; }
    public string AccountDealDate { get; set; }
    public string AccountUpdateDate { get; set; }
    public int MetegDoarNet { get; set; }
    public int KodHarshaatPeilut { get; set; }
    public int AccountClosingReasonCode { get; set; }
    public string ProductLabel { get; set; }
    public string AccountAgreementOpeningDate { get; set; }
  }
}
