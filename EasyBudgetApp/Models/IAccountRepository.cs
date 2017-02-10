using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Models
{
  public interface IAccountRepository
  {
    IObservable<IAccount> GetAccounts(IEnumerable<IDataProvider> dataProviders);
  }
}
