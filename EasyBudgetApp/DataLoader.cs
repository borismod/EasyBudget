using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Threading;
using DataProvider;
using DataProvider.Hapoalim;
using DataProvider.Interfaces;
using EasyBudgetApp.Filters;
using EasyBudgetApp.Models;
using EasyBudgetApp.Models.Credentials;
using EasyBudgetApp.ViewModels;
using log4net;

namespace EasyBudgetApp
{
    public class DataLoader
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserProfileManager ProfileManager { get; set; }

        private event EventHandler<AccountEventArgs> AccountLoading;
        private event EventHandler<AccountErrorEventArgs> AccountLoadingError;
        private event EventHandler<AccountsEventArgs> AccountsLoadingCompleted;
        private event EventHandler<TransactionEventArgs> TransactionLoaded;
        private event EventHandler<AccountEventArgs> AccountLoaded;

        public IObservable<IAccount> WhenAccountLoading
        {
            get
            {
                return Observable.FromEventPattern<AccountEventArgs>(
                  h => AccountLoading += h,
                  h => AccountLoading -= h)
                  .Select(x => x.EventArgs.Account);
            }
        }
        public IObservable<DataProviderException> WhenAccountLoadingError
        {
            get
            {
                return Observable.FromEventPattern<AccountErrorEventArgs>(
                  h => AccountLoadingError += h,
                  h => AccountLoadingError -= h)
                  .Select(x => x.EventArgs.Error);
            }
        }
        public IObservable<IList<IAccount>> WhenAccountsLoadingCompleted
        {
            get
            {
                return Observable.FromEventPattern<AccountsEventArgs>(
                  h => AccountsLoadingCompleted += h,
                  h => AccountsLoadingCompleted -= h)
                  .Select(x => x.EventArgs.Accounts);
            }
        }
        public IObservable<TransactionViewModel> WhenTransactionLoaded
        {
            get
            {
                return Observable.FromEventPattern<TransactionEventArgs>(
                  h => TransactionLoaded += h,
                  h => TransactionLoaded -= h)
                  .Select(x => x.EventArgs.Transaction);
            }
        }
        public IObservable<IAccount> WhenAccountLoaded
        {
            get
            {
                return Observable.FromEventPattern<AccountEventArgs>(
                  h => AccountLoaded += h,
                  h => AccountLoaded -= h)
                  .Select(x => x.EventArgs.Account);
            }
        }

        public DataLoader(UserProfileManager profileManager)
        {
            ProfileManager = profileManager;
        }
        public void LoadAsync()
        {
            var providers = ProvidersFactory.CreateProviders(ProfileManager.DataProviders);
            var accounts = AccountsFactory.GetAccounts(providers, IsInProfile);
            LoadAccounts(accounts);
        }
        public void LoadAccount(IAccount account, DateTime firstDayOfMonth, DateTime lastDayOfMonth)
        {
            if (account == null)
            {
                return;
            }

            OnNewAccountLoading(account);
            LoadTransactions(account, firstDayOfMonth, lastDayOfMonth);
        }
        private void LoadAccounts(IList<IObservable<IAccount>> accounts)
        {
            foreach (var accountsInCompany in accounts)
            {
                var loadingAccounts = new List<IAccount>();
                accountsInCompany.ObserveOn(new DispatcherSynchronizationContext()).Subscribe((account) =>
                {
                    loadingAccounts.Add(account);
                    DateTime now = DateTime.Now;
                    var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    LoadAccount(account, firstDayOfMonth, lastDayOfMonth);
                },
                (error) =>
                {
                    OnAccountLoadingError((DataProviderException)error);
                },
                () =>
                {
                    OnAccountsLoadingCompleted(loadingAccounts);
                });
            }
        }
        private void LoadTransactions(IAccount account, DateTime firstDayOfMonth, DateTime lastDayOfMonth)
        {
            IObservable<IEnumerable<ITransaction>> observableTransactions = AccountsFactory.GetTransations(account, firstDayOfMonth, lastDayOfMonth);
            observableTransactions?.ObserveOn(new DispatcherSynchronizationContext()).Subscribe((transactions) =>
            {
                if (transactions == null)
                {
                    return;
                }

                var list = transactions as List<ITransaction> ?? transactions.ToList();
                list = list.OrderBy(t => t.EventDate).ToList();

                DefaultActivityFilter(list);

                foreach (var transaction in list)
                {
                    var transactionViewModel = new TransactionViewModel(transaction);
                    OnNewTransactionLoaded(transactionViewModel);
                }

                OnNewAccountLoaded(account);
            },
            (error) =>
            {

            });
        }

        private static void DefaultActivityFilter(List<ITransaction> list)
        {
            foreach (var transaction in list)
            {
                if (transaction.Type == TransactionType.Expense &&
                    (transaction.SupplierId.StartsWith(ServiceProviders.PoalimExpress) ||
                     transaction.SupplierId.StartsWith(ServiceProviders.Visa)))
                {
                    InactiveTransactionsManager.SetActivity(transaction.EventId, false);
                }
            }
        }

        private void OnNewAccountLoading(IAccount account)
        {
            EventHandler<AccountEventArgs> eventHandler = AccountLoading;
            eventHandler?.Invoke(this, new AccountEventArgs(account));
        }
        private void OnAccountsLoadingCompleted(IList<IAccount> accounts)
        {
            EventHandler<AccountsEventArgs> eventHandler = AccountsLoadingCompleted;
            eventHandler?.Invoke(this, new AccountsEventArgs(accounts));
        }
        private void OnNewTransactionLoaded(TransactionViewModel transaction)
        {
            EventHandler<TransactionEventArgs> eventHandler = TransactionLoaded;
            eventHandler?.Invoke(this, new TransactionEventArgs(transaction));
        }
        private void OnNewAccountLoaded(IAccount account)
        {
            EventHandler<AccountEventArgs> eventHandler = AccountLoaded;
            eventHandler?.Invoke(this, new AccountEventArgs(account));
        }
        private void OnAccountLoadingError(DataProviderException error)
        {
            EventHandler<AccountErrorEventArgs> eventHandler = AccountLoadingError;
            eventHandler?.Invoke(this, new AccountErrorEventArgs(error));
        }

        private bool IsInProfile(IAccount account)
        {
            foreach (var profile in ProfileManager.UserProfiles)
            {
                foreach (var accountProfile in profile.Accounts)
                {
                    if (account.AccountId == accountProfile.Id)
                        return true;
                }    
            }
            
            return false;
        }

        private class AccountEventArgs : EventArgs
        {
            internal IAccount Account { get; }

            internal AccountEventArgs(IAccount account)
            {
                Account = account;
            }
        }
        private class AccountsEventArgs : EventArgs
        {
            internal IList<IAccount> Accounts { get; }

            internal AccountsEventArgs(IList<IAccount> accounts)
            {
                Accounts = accounts;
            }
        }
        private class TransactionEventArgs : EventArgs
        {
            internal TransactionViewModel Transaction { get; }

            internal TransactionEventArgs(TransactionViewModel transaction)
            {
                Transaction = transaction;
            }
        }
        private class AccountErrorEventArgs : EventArgs
        {
            internal DataProviderException Error { get; }

            internal AccountErrorEventArgs(DataProviderException error)
            {
                Error = error;
            }
        }
    }
}
