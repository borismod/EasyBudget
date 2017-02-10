using System.Collections.Generic;

namespace DataProvider.Hapoalim.Responses
{
  public class TransactionsResponse
  {
    public RetrievalTransaction RetrievalTransactionData { get; set; }
    public object Message { get; set; }
    public IList<TransactionResponse> Transactions { get; set; }

    public TransactionsResponse()
    {
      Transactions = new List<TransactionResponse>();
      RetrievalTransactionData = new RetrievalTransaction();
    }

    public class RetrievalTransaction
    {
      public object BalanceAmountDisplayIndication { get; set; }
      public int BranchNumber { get; set; }
      public long AccountNumber { get; set; }
      public long RetrievalMinDate { get; set; }
      public long RetrievalMaxDate { get; set; }
      public int RetrievalStartDate { get; set; }
      public int RetrievalEndDate { get; set; }
      public int EventCounter { get; set; }
      public bool JoinPfm { get; set; }
    }
  }
}
