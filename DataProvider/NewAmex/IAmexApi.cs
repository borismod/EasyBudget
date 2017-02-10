using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider.Amex.Responses;

namespace DataProvider.Amex
{
  public interface IAmexApi : IDisposable
  {
    bool IsReady { get; }
    CardListResponse GetCards();
    IEnumerable<DealResponse> GetTransactions(long cardIndex);
  }
}
