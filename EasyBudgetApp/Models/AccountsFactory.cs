using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DataProvider;
using DataProvider.Interfaces;
using log4net;

namespace EasyBudgetApp.Models
{
    public class AccountsFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly IDictionary<IAccount, IDataProvider> ProvidersMap = new Dictionary<IAccount, IDataProvider>();

        public static IObservable<IAccount> GetAccounts(IObservable<IDataProvider> providers, Predicate<IAccount> filter)
        {
            return Observable.Create<IAccount>((observer) => WhenNewProviderArrives(providers, observer, filter));
        }

        public static IList<IObservable<IAccount>> GetAccounts(IList<IObservable<IDataProvider>> providers, Predicate<IAccount> filter)
        {
            IList<IObservable<IAccount>> accounts = new List<IObservable<IAccount>>();
            foreach (var provider in providers)
            {
                accounts.Add(Observable.Create<IAccount>((observer) => WhenNewProviderArrives(provider, observer, filter)));
            }

            return accounts;
        }

        public static IObservable<IEnumerable<ITransaction>> GetTransations(IAccount account, DateTime startDate, DateTime endDate)
        {
            IDataProvider provider;
            ProvidersMap.TryGetValue(account, out provider);
            if (provider == null)
            {
                return null;
            }
            // check provider for Null and return onError (?)

            var result = Observable.Create<IEnumerable<ITransaction>>((observer) =>
            {
                var task = new Task(() =>
                  {
                      try
                      {
                          var transactions = provider.GetTransactions(account.AccountId, startDate, endDate);
                          observer.OnNext(transactions);
                      }
                      catch (Exception ex)
                      {
                          observer.OnError(ex);
                      }
                      
                      observer.OnCompleted();
                  });

                task.Start();

                return () => { };
            });

            return result;
        }

        private static IDisposable WhenNewProviderArrives(IObservable<IDataProvider> providers, IObserver<IAccount> observer, Predicate<IAccount> filter)
        {
            return providers.AsObservable().Subscribe((provider) =>
            {
                _log.DebugFormat("New provider is created - {0}", provider.Name);
                var task = new Task(() =>
                {
                    var accounts = provider.GetAccounts();
                    foreach (var account in accounts)
                    {
                        if (filter(account))
                        {
                            _log.InfoFormat("New account created - {0} : {1}", provider.Name, account.AccountId);
                            ProvidersMap.Add(account, provider);
                            observer.OnNext(account);
                        }
                    }
                    observer.OnCompleted();
                });
                task.Start();
            },
            (error) =>
            {
                _log.ErrorFormat("Cannot create provider - {0}. \n Deatils: {1}", error.Message, error.StackTrace);
                observer.OnError(error);
            });
        }

    }
}
