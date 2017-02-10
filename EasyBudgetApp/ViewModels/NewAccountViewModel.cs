using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Security;
using System.Windows.Threading;
using DataProvider;
using DataProvider.Interfaces;
using EasyBudgetApp.Models;
using EasyBudgetApp.Models.Credentials;

namespace EasyBudgetApp.ViewModels
{
    public class NewAccountViewModel : BaseViewModel
    {
        private ObservableCollection<CompanyViewModel> _companies;
        public ObservableCollection<CompanyViewModel> Companies
        {
            get
            {
                return _companies;
            }
            set
            {
                _companies = value;
                OnPropertyChanged(() => Companies);
            }
        }

        private ObservableCollection<AccountViewModel> _newAccounts;
        public ObservableCollection<AccountViewModel> NewAccounts
        {
            get
            {
                return _newAccounts;
            }
            set
            {
                _newAccounts = value;
                OnPropertyChanged(() => NewAccounts);
            }
        }

        private IDictionary<IAccount, AccountViewModel> _accountMap;

        private CompanyViewModel _selectedCompany;
        public CompanyViewModel SelectedCompany
        {
            get
            {
                return _selectedCompany ?? CompanyViewModel.Empty;
            }

            set
            {
                if (_selectedCompany == value)
                {
                    return;
                }

                Error
                  = string.Empty;
                NewAccounts.Clear();
                _selectedCompany = value;
                OnPropertyChanged(() => SelectedCompany);
            }
        }

        private SecureString _securePassword;
        public SecureString SecurePassword
        {
            get
            {
                return _securePassword;
            }

            set
            {
                _securePassword = value;
                OnPropertyChanged(() => SecurePassword);
            }
        }

        private RelayCommand _showAccountsCommand;
        public RelayCommand ShowAccountsCommand
        {
            get
            {
                return _showAccountsCommand;
            }

            set
            {
                _showAccountsCommand = value;
                OnPropertyChanged(() => ShowAccountsCommand);
            }
        }

        private RelayCommand _addAccountsCommand;
        public RelayCommand AddAccountsCommand
        {
            get
            {
                return _addAccountsCommand;
            }

            set
            {
                _addAccountsCommand = value;
                OnPropertyChanged(() => AddAccountsCommand);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }

        private string _error;
        public string Error
        {
            get
            {
                return _error;
            }

            set
            {
                _error = value;
                OnPropertyChanged(() => Error);
            }
        }

        private static event EventHandler<ProfileEventArgs> NewProfileAdded;
        public static IObservable<UserProfile> WhenNewProfileAdded
        {
            get
            {
                return Observable.FromEventPattern<ProfileEventArgs>(
                        h => NewProfileAdded += h,
                        h => NewProfileAdded -= h)
                    .Select(x => x.EventArgs.Profile);
            }
        }

        public NewAccountViewModel(DataLoader dataLoader)
        {
            var companiesList = dataLoader.ProfileManager.CompanyProfiles.Select(profile => new CompanyViewModel(profile)).ToList();
            var sortedCompaniesList = from company in companiesList
                                      orderby company.Name
                                      select company;

            Companies = new ObservableCollection<CompanyViewModel>(sortedCompaniesList);
            NewAccounts = new ObservableCollection<AccountViewModel>();
            _accountMap = new Dictionary<IAccount, AccountViewModel>();

            ShowAccountsCommand = new RelayCommand((o) =>
            {
                if (SelectedCompany != null)
                {
                    HandleNewAccount();
                }
            });

            AddAccountsCommand = new RelayCommand((o) =>
            {
                OnNewProfileAdded(ToUserProfile());
                foreach (var newAccount in NewAccounts.Where(newAccount => newAccount.IsEnabled))
                {
                    var account = _accountMap.FirstOrDefault((p) => p.Key.AccountId == newAccount.Id).Key;

                    DateTime now = DateTime.Now;
                    var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    dataLoader.LoadAccount(account, firstDayOfMonth, lastDayOfMonth);
                }

                NewAccounts.Clear();
            });
        }

        private UserProfile ToUserProfile()
        {
            var newProfile = new UserProfile
            {
                InstitutionName = SelectedCompany.Name,
                InstitutionType = string.Empty,
                Credentials = BuildCredentialDetails(),
                Accounts = new List<UserAccountProfile>()
            };

            foreach (var newAccount in NewAccounts)
            {
                if (newAccount.IsEnabled)
                {
                    newProfile.Accounts.Add(newAccount.ToUserAccountProfile());
                }
            }

            if (newProfile.Accounts.Any())
            {
                newProfile.UserId = newProfile.Accounts.First().UserId;
            }
            
            return newProfile;
        }

        protected void OnNewProfileAdded(UserProfile profile)
        {
            EventHandler<ProfileEventArgs> eventHandler = NewProfileAdded;
            eventHandler?.Invoke(this, new ProfileEventArgs(profile));
        }

        private void HandleNewAccount()
        {
            var credentials = BuildCredentialDetails();
            if (!credentials.Any(nameValue => string.IsNullOrEmpty(nameValue.Value)))
            {
                IsBusy = true;
                NewAccounts.Clear();

                var observableProvider = ProvidersFactory.CreateProvider(new ProviderDescriptor(SelectedCompany.Name, credentials, null));
                var accounts = AccountsFactory.GetAccounts(observableProvider, (a) => true);
                accounts.ObserveOn(new DispatcherSynchronizationContext()).Subscribe((account) =>
                {
                    var newAccount = new AccountViewModel(account);
                    _accountMap.Add(account, newAccount);
                    NewAccounts.Add(newAccount);
                },
                (error) =>
                {
                    Error = error.Message;
                    IsBusy = false;
                },
                () =>
                {
                    IsBusy = false;
                });
            }
        }

        private Dictionary<string, string> BuildCredentialDetails(string passwordFieldName = "Password")
        {
            var fields = SelectedCompany.CredentialFields.ToDictionary(field => field.Name, field => field.Value);
            if (fields.ContainsKey(passwordFieldName))
            {
                fields[passwordFieldName] = SecurePassword?.ConvertToUnsecureString() ?? string.Empty;
            }
            return fields;
        }

        private class AccounEventArgs : EventArgs
        {
            internal AccountViewModel Account { get; }

            internal AccounEventArgs(AccountViewModel account)
            {
                Account = account;
            }
        }

        public class ProfileEventArgs : EventArgs
        {
            internal UserProfile Profile { get; }

            internal ProfileEventArgs(UserProfile profile)
            {
                Profile = profile;
            }
        }
    }
}

