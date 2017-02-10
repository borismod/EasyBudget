using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DataProvider.Hapoalim;
using DataProvider.Interfaces;
using DataProvider.Model;
using EasyBudgetApp.Models;
using EasyBudgetApp.Models.Credentials;

namespace EasyBudgetApp.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        #region Properties
        private string _companyName;
        public string CompanyName
        {
            get
            {
                return _companyName;
            }
            set
            {
                _companyName = value;
                OnPropertyChanged(() => CompanyName);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }

        private string _userId;
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                OnPropertyChanged(() => UserId);
            }
        }

        private long _id;
        public long Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(() => Id);
            }
        }

        private string _details;
        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged(() => Details);
            }
        }

        private double _pereodic;
        public double Pereodic
        {
            get
            {
                return _pereodic;
            }
            set
            {
                _pereodic = value;
                OnPropertyChanged(() => Pereodic);
            }
        }

        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(() => Total);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(() => IsEnabled);
                    OnIsEnabledChanged(this);
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged(() => Error);
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(() => ErrorMessage);
            }
        }

        private int _currentMonth;
        public int CurrentMonth
        {
            get { return _currentMonth; }
            set
            {
                _currentMonth = value;
                OnPropertyChanged(() => CurrentMonth);
            }
        }

        private AccountType _type;
        public AccountType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(() => Type);
            }
        }

        public IAccount Account { get; private set; }

        private ObservableCollection<TransactionViewModel> _transactions;
        public ObservableCollection<TransactionViewModel> Transactions
        {
            get { return _transactions; }
            set
            {
                _transactions = value;
                OnPropertyChanged(() => Transactions);
            }
        }
        #endregion Properties

        private static event EventHandler<AccountEventArgs> IsEnabledChanged;
        public static IObservable<AccountViewModel> WhenShowNewAccountExecuted
        {
            get
            {
                return Observable.FromEventPattern<AccountEventArgs>(
                        h => IsEnabledChanged += h,
                        h => IsEnabledChanged -= h)
                    .Select(x => x.EventArgs.Account);
            }
        }
       
        private AccountViewModel()
        {
            Transactions = new ObservableCollection<TransactionViewModel>();
            Type = AccountType.None;
        }

        public AccountViewModel(UserAccountProfile account, DataLoader dataLoader) : this()
        {
            CompanyName = account.InstitutionName;
            Id = account.Id;
            Name = account.Name;
            Details = account.Details;
            UserId = account.UserId;
            Pereodic = double.NaN;
            Total = double.NaN;
            IsEnabled = account.IsEnabled;

            HandleNewTransaction(dataLoader);
            HandleAccountLoaded(dataLoader);
        }

        public AccountViewModel(IAccount account, DataLoader dataLoader = null) : this()
        {
            SetAccountData(account);
            IsEnabled = true;

            if (dataLoader != null)
            {
                HandleAccountLoaded(dataLoader);
                HandleNewTransaction(dataLoader);
            }
        }

        private void HandleNewTransaction(DataLoader dataLoader)
        {
            IsLoading = true;
            dataLoader.WhenTransactionLoaded.Where((t) => t.AccountId == Id).Subscribe((transaction) =>
            {
                Transactions.Add(transaction);
                HandleTransactionActivityChange(transaction);

                if (!transaction.IsActive)
                {
                    return;
                }

                AddToPeriodic(transaction);
            });
        }

        private void HandleTransactionActivityChange(TransactionViewModel transaction)
        {
            transaction.WhenTransactionActivityChanged.Subscribe((t) =>
            {
                if (transaction.IsActive)
                {
                    AddToPeriodic(transaction);
                }
                else
                {
                    RemoveFromPeriodic(transaction);
                }

                SetParentActivity(transaction, transaction.IsActive);
            });
        }

        private void SetParentActivity(TransactionViewModel transaction, bool isActive)
        {
            if (Transactions.All(pt => pt.ParentId != transaction.ParentId))
            {
                var parent = _transactions.FirstOrDefault(pt => pt.Id == transaction.ParentId);
                if (parent != null)
                {
                    parent.IsActive = isActive;

                    if (isActive)
                    {
                        parent.Amount += transaction.Amount;
                    }
                    else
                    {
                        parent.Amount -= transaction.Amount;
                    }
                }
            }
        }
        private void AddToPeriodic(TransactionViewModel transaction)
        {
            if (transaction.Type == TransactionType.Income)
            {
                Pereodic = double.IsNaN(Pereodic) ? transaction.Amount : Pereodic + transaction.Amount;
            }
            else
            {
                Pereodic = double.IsNaN(Pereodic) ? -1 * transaction.Amount : Pereodic - transaction.Amount;
            }
        }

        private void RemoveFromPeriodic(TransactionViewModel transaction)
        {
            if (transaction.Type == TransactionType.Income)
            {
                Pereodic = double.IsNaN(Pereodic) ? -1 * transaction.Amount : Pereodic - transaction.Amount;
            }
            else
            {
                Pereodic = double.IsNaN(Pereodic) ? transaction.Amount : Pereodic + transaction.Amount;
            }
        }

        private void HandleAccountLoaded(DataLoader dataLoader)
        {
            dataLoader.WhenAccountLoaded.Where((ac) => ac.AccountId == Id).Subscribe((ac) =>
            {
                IsLoading = false;
            });
        }

        public void HandleLoadingError(string message)
        {
            IsLoading = false;
            Error = "Error";
            ErrorMessage = string.IsNullOrEmpty(message) ? "No error details" : message;
        }

        public void Update(IAccount account)
        {
            SetAccountData(account);
        }

        public void RemoveAllTransactions()
        {
            Pereodic = double.NaN;
            Transactions.Clear();
        }

        private void SetAccountData(IAccount account)
        {
            Account = account;

            if (account is BankAccount)
            {
                var data = (BankAccount)account;
                Id = data.AccountId;
                CompanyName = data.BankName;
                UserId = data.UserId;
                Name = data.AccountNumber.ToString();
                Details = $"{data.BranchNumber}-{data.AccountNumber}";
                Total = data.Balance;
                Type = AccountType.Bank;
            }
            else if (account is CreditCardAccount)
            {
                var data = (CreditCardAccount)account;
                Id = data.AccountId;
                CompanyName = data.CompanyName;
                UserId = data.UserId;
                Name = data.CardName;
                Details = $"{data.CardName}-{data.CardNumber}";
                Total = data.Balance;
                Type = AccountType.Credit;
            }
            else
            {
                Name = account.Label;
                CompanyName = account.Label;
                UserId = account.UserId;
                Id = account.AccountId;
                Details = account.Label;
                Total = account.Balance;
                Type = AccountType.None;
            }
        }

        public UserAccountProfile ToUserAccountProfile()
        {
            var result = new UserAccountProfile
            {
                Id = Id,
                Name = Name,
                Details = Details,
                IsEnabled = IsEnabled,
                InstitutionName = CompanyName,
                UserId = UserId
            };

            return result;
        }

        protected virtual void OnIsEnabledChanged(AccountViewModel account)
        {
            var enevtArgs = new AccountEventArgs(account);
            IsEnabledChanged?.Invoke(this, enevtArgs);
        }

        public override bool Equals(object obj)
        {
            var profile = obj as AccountViewModel;
            if (profile == null)
            {
                return false;
            }

            return Equals(profile);
        }

        public bool Equals(AccountViewModel viewModel)
        {
            return Id == viewModel?.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private class AccountEventArgs
        {
            internal AccountViewModel Account { get; }

            internal AccountEventArgs(AccountViewModel account)
            {
                Account = account;
            }
        }
    }
}
