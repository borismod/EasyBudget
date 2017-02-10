using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using EasyBudgetApp.Models.Credentials;

namespace EasyBudgetApp.ViewModels
{
    public class AccountsViewModel : AccountsBaseViewModel
    {
        private event EventHandler<EventArgs> ShowNewAccountExecuted;
        public IObservable<EventArgs> WhenShowNewAccountExecuted
        {
            get
            {
                return Observable.FromEventPattern<EventArgs>(
                        h => ShowNewAccountExecuted += h,
                        h => ShowNewAccountExecuted -= h)
                    .Select(x => x.EventArgs);
            }
        }
        private AccountViewModel _selectedAccount;
        public AccountViewModel SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                _selectedAccount = value;
                OnPropertyChanged(() => SelectedAccount);
            }
        }
        public ICommand ShowAddNewAccountLayoutCommand { set; get; }
        public AccountsViewModel(DataLoader dataLoader, Auditor auditor, DateStateViewModel dateState) : base(dataLoader, auditor, dateState)
        {
            CreateCommands();
        }
        protected override void LoadAccountsFromProfile(IEnumerable<UserProfile> userProfile, DataLoader dataLoader)
        {
            base.LoadAccountsFromProfile(userProfile, dataLoader);

            var accountViewModel = Accounts.FirstOrDefault();
            if (accountViewModel != null)
            {
                accountViewModel.IsSelected = true;
            }
        }
        private void CreateCommands()
        {
            ShowAddNewAccountLayoutCommand = new RelayCommand((o) =>
            {
                OnShowNewAccountExecuted();
            });
        }
        private void OnShowNewAccountExecuted()
        {
            ShowNewAccountExecuted?.Invoke(null, EventArgs.Empty);
        }
        protected override void HandleDateStateChange(DateStateViewModel dateState)
        {
            base.HandleDateStateChange(dateState);
            dateState.WhenDateStateChanged.Subscribe((state) =>
            {
                foreach (var accountVm in Accounts)
                {
                    accountVm.RemoveAllTransactions();
                    if (accountVm.Account != null)
                    {
                        accountVm.IsLoading = true;
                    }
                }
            });
        }
    }
}
