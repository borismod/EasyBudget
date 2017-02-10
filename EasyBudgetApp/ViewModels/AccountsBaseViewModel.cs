using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DataProvider;
using EasyBudgetApp.Models.Credentials;
using log4net;

namespace EasyBudgetApp.ViewModels
{
    public class AccountsBaseViewModel : BaseViewModel
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Properties
        private ObservableCollection<AccountViewModel> _accounts;
        public ObservableCollection<AccountViewModel> Accounts
        {
            get { return _accounts; }
            set
            {
                _accounts = value;
                OnPropertyChanged(() => Accounts);
            }
        }

        protected IDictionary<long, AccountViewModel> AccountsById;

        private DateStateViewModel _dateStateVm;

        public DateStateViewModel DateStateVm
        {
            get
            {
                return _dateStateVm;
            }
            set
            {
                _dateStateVm = value;
                OnPropertyChanged(() => DateStateVm);
            }
        }
        protected int _navigationTitle;
        public int NavigationTitle
        {
            get { return _navigationTitle; }
            set
            {
                _navigationTitle = value;
                OnPropertyChanged(() => NavigationTitle);
            }
        }

        protected double _monthIncome;
        public double MonthIncome
        {
            get { return _monthIncome; }
            set
            {
                _monthIncome = value;
                OnPropertyChanged(() => MonthIncome);
            }
        }

        protected double _monthExpenses;
        public double MonthExpenses
        {
            get { return _monthExpenses; }
            set
            {
                _monthExpenses = value;
                OnPropertyChanged(() => MonthExpenses);
            }
        }

        protected double _monthBalance;
        public double MonthBalance
        {
            get { return _monthBalance; }
            set
            {
                _monthBalance = value;
                OnPropertyChanged(() => MonthBalance);
            }
        }

        protected bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

        private RelayCommand _removeAccount;
        public RelayCommand RemoveAccount
        {
            get
            {
                return _removeAccount;
            }

            set
            {
                _removeAccount = value;
                OnPropertyChanged(() => RemoveAccount);
            }
        }

        private bool _disposed;

        protected Auditor _auditor;
        protected DataLoader _dataLoader;
        #endregion Properties

        protected AccountsBaseViewModel(DataLoader dataLoader, Auditor auditor, DateStateViewModel dateState)
        {
            Accounts = new ObservableCollection<AccountViewModel>();
            AccountsById = new Dictionary<long, AccountViewModel>();
            _auditor = auditor;
            _dataLoader = dataLoader;
            _dateStateVm = dateState;
            
            HandleAuditorNotifications(auditor);
            LoadAccountsFromProfile(dataLoader.ProfileManager.UserProfiles, dataLoader);
            HandleAccountLoading(dataLoader);
            HandleDateStateChange(dateState);

            RemoveAccount = new RelayCommand(a =>
            {
                var accountVm = a as AccountViewModel;
                if (accountVm == null)
                {
                    return;
                }

                Accounts.Remove(accountVm);

                _dataLoader.ProfileManager.RemoveUserAccountProfile(accountVm.ToUserAccountProfile());
            });
        }

        protected void HandleAuditorNotifications(Auditor auditor)
        {
            auditor.WhenStateUpdated.Subscribe((state) =>
            {
                MonthIncome = state.MonthIncome;
                MonthExpenses = state.MonthExpenses;
                MonthBalance = state.MonthBalance;
            });
        }

        protected virtual void LoadAccountsFromProfile(IEnumerable<UserProfile> userProfile, DataLoader dataLoader)
        {
            foreach (var account in userProfile.SelectMany(profile => profile.Accounts))
            {
                var accountVm = new AccountViewModel(account, dataLoader);
                if (!AccountsById.Keys.Contains(account.Id))
                {
                    AccountsById.Add(account.Id, accountVm);
                    Accounts.Add(accountVm);
                }
            }
        }

        protected void HandleAccountLoading(DataLoader dataLoader)
        {
            if (dataLoader.ProfileManager.UserProfiles.Count > 0)
            {
                IsLoading = true;
            }

            dataLoader.WhenAccountLoading.Subscribe((account) =>
            {
                if (account == null)
                {
                    return;
                }
                _log.DebugFormat("Account {0} is loading", account.AccountId);

                AccountViewModel accountVm;
                AccountsById.TryGetValue(account.AccountId, out accountVm);

                if (accountVm == null)
                {
                    accountVm = new AccountViewModel(account, dataLoader);
                    AccountsById.Add(account.AccountId, accountVm);
                    Accounts.Add(accountVm);
                }
                else
                {
                    accountVm.Update(account);
                }
            });

            dataLoader.WhenAccountLoaded.Subscribe((account) =>
            {
                _log.DebugFormat("Account {0} is loaded", account.AccountId);

                var loadingAccounts = Accounts.Where((ac) => ac.IsLoading).ToArray();
                if (!loadingAccounts.Any() || (loadingAccounts.Count() == 1 && loadingAccounts.FirstOrDefault().Id == account.AccountId))
                {
                    IsLoading = false;
                }
            });

            dataLoader.WhenAccountLoadingError.Subscribe((error) =>
            {
                foreach (var account in error.ProviderDescriptor.Accounts)
                {
                    AccountViewModel accountVm;
                    AccountsById.TryGetValue(account.AccountId, out accountVm);
                    if (accountVm != null)
                    {
                        _log.ErrorFormat("Error on loading account {0}", accountVm.Id);
                        accountVm.HandleLoadingError(error.Message);
                    }
                }
            });

            dataLoader.WhenAccountsLoadingCompleted.Subscribe((loadedAccounts) =>
            {
                var first = loadedAccounts.FirstOrDefault();
                if (first == null)
                {
                    return;
                }

                // Elimenate accounts that exist but are not being loaded
                foreach (var accountKey in AccountsById.Keys)
                {
                    var storedAccount = AccountsById[accountKey];
                    if (storedAccount.UserId != first.UserId ||
                        ProvidersFactory.GetVendorIdByName(storedAccount.CompanyName) != first.VendorId)
                    {
                        continue;
                    }
                    
                    if (loadedAccounts.All(a => a.AccountId != storedAccount.Id))
                    {
                        storedAccount.HandleLoadingError("Cannot access");
                    }
                }
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        protected virtual void HandleDateStateChange(DateStateViewModel dateState)
        {
            dateState.WhenDateStateChanged.Subscribe((state) =>
            {
                IsLoading = true;
            });
        }
    }
}
