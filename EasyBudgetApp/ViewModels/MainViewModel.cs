using System;

namespace EasyBudgetApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private ToolbarViewModel _toolbar;
        public ToolbarViewModel Toolbar
        {
            get
            {
                return _toolbar;
            }
            set
            {
                _toolbar = value;
                OnPropertyChanged(() => Toolbar);
            }
        }

        private BaseViewModel _currentContent;
        public BaseViewModel CurrentContent
        {
            get
            {
                return _currentContent;
            }
            set
            {
                _currentContent = value;
                OnPropertyChanged(() => CurrentContent);
            }
        }

        private NewAccountViewModel _newAccount;
        public NewAccountViewModel NewAccount
        {
            get
            {
                return _newAccount;
            }
            set
            {
                _newAccount = value;
                OnPropertyChanged(() => NewAccount);
            }
        }

        private NewCategoryViewModel _newCategory;
        public NewCategoryViewModel NewCategory
        {
            get
            {
                return _newCategory;
            }
            set
            {
                _newCategory = value;
                OnPropertyChanged(() => NewCategory);
            }
        }

        private bool _showAddNewAccountLayout;
        public bool ShowAddNewAccountLayout
        {
            get
            {
                return _showAddNewAccountLayout;
            }
            set
            {
                _showAddNewAccountLayout = value;
                OnPropertyChanged(() => ShowAddNewAccountLayout);
            }
        }

        private bool _showAddNewCategoryLayout;
        public bool ShowAddNewCategoryLayout
        {
            get
            {
                return _showAddNewCategoryLayout;
            }
            set
            {
                _showAddNewCategoryLayout = value;
                OnPropertyChanged(() => ShowAddNewCategoryLayout);
            }
        }

        public MainViewModel(DataLoader dataLoader, ToolbarViewModel toolbarVm, AccountsViewModel accountsVm, NewAccountViewModel newAccountVm, NewCategoryViewModel newCategoryVm, BudgetViewModel budgetVm, DateStateViewModel dateState)
        {
            Toolbar = toolbarVm;
            NewAccount = newAccountVm;
            NewCategory = newCategoryVm;

            accountsVm.WhenShowNewAccountExecuted.Subscribe((e) =>
            {
                ShowAddNewAccountLayout = true;
            });

            budgetVm.WhenShowNewCategoryExecuted.Subscribe((e) =>
            {
                ShowAddNewCategoryLayout = true;
            });

            HandleDateStateChange(dateState, dataLoader, accountsVm);
        }

        private void HandleDateStateChange(DateStateViewModel dateState, DataLoader dataLoader, AccountsViewModel accountsVm)
        {
            dateState.WhenDateStateChanged.Subscribe((state) =>
            {
                var firstDayOfMonth = new DateTime(state.Year, state.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                foreach (var accountVm in accountsVm.Accounts)
                {
                    dataLoader.LoadAccount(accountVm.Account, firstDayOfMonth, lastDayOfMonth);
                }
            });
        }
    }
}
