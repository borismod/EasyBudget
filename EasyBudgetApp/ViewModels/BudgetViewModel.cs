using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Reactive.Linq;
using DataProvider.Interfaces;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
    public class BudgetViewModel : AccountsBaseViewModel
    {
        private ObservableCollection<TransactionViewModel> _transactions;
        public ObservableCollection<TransactionViewModel> Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                OnPropertyChanged(() => Transactions);
            }
        }

        private ObservableCollection<BudgetCategoryViewModel> _categories;
        public ObservableCollection<BudgetCategoryViewModel> Categories
        {
            get
            {
                return _categories;
            }

            set
            {
                _categories = value;
                OnPropertyChanged(() => Categories);
            }
        }

        private BudgetCategoryViewModel _selectedCategory;
        public BudgetCategoryViewModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    TransactionsView.Refresh();
                    OnPropertyChanged(() => SelectedCategory);
                }
            }
        }

        public ICollectionView TransactionsView { get; }

        public ICommand ShowAddNewCategoryLayoutCommand { set; get; }
        private event EventHandler<EventArgs> ShowNewCategoryExecuted;
        public IObservable<EventArgs> WhenShowNewCategoryExecuted
        {
            get
            {
                return Observable.FromEventPattern<EventArgs>(
                        h => ShowNewCategoryExecuted += h,
                        h => ShowNewCategoryExecuted -= h)
                    .Select(x => x.EventArgs);
            }
        }
        public BudgetViewModel(DataLoader dataLoader, Auditor auditor, DateStateViewModel dateState, CategoriesViewModel categories)
            : base(dataLoader, auditor, dateState)
        {
            Transactions = new ObservableCollection<TransactionViewModel>();
            CreateCommands();
            PopulateCategories(categories);
            HandleExpenseTransactions(dataLoader);

            TransactionsView = CollectionViewSource.GetDefaultView(Transactions);
            TransactionsView.Filter = TransactionsFilter;
            TransactionsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
        }
        protected override void HandleDateStateChange(DateStateViewModel dateState)
        {
            base.HandleDateStateChange(dateState);
            dateState.WhenDateStateChanged.Subscribe((state) =>
            {
                Transactions.Clear();
                foreach (var category in Categories)
                {
                    category.Spent = 0;
                }
            });
        }
        private void PopulateCategories(CategoriesViewModel categories)
        {
            Categories = new ObservableCollection<BudgetCategoryViewModel>();
            foreach (var category in categories.Categories)
            {
                if (!string.IsNullOrEmpty(category.Name))
                {
                    Categories.Add(new BudgetCategoryViewModel(category.Id, category.Name));
                }
            }
        }
        private void HandleExpenseTransactions(DataLoader dataLoader)
        {
            dataLoader.WhenTransactionLoaded.Subscribe((transaction) =>
            {
                if (transaction.Type == TransactionType.Expense)
                {
                    HandleAgregatedTransaction(transaction);
                    Transactions.Add(transaction);
                    HandleTransactionActivityChange(transaction);
                    TransactionsView.Refresh();

                    var category = GetCategoryById(transaction.Category);
                    if (category != null && transaction.IsActive)
                    {
                        category.Spent += transaction.Amount;
                    }
                }
            });
        }
        private void CreateCommands()
        {
            ShowAddNewCategoryLayoutCommand = new RelayCommand((o) =>
            {
                OnShowNewCategoryExecuted();
            });
        }
        private void HandleAgregatedTransaction(TransactionViewModel transaction)
        {
            if (Transactions.Count(t => t.SupplierId == transaction.SupplierId) == 1)
            {
                var child = Transactions.First(t => t.SupplierId == transaction.SupplierId);

                var newParent = new AgregatedTransaction
                {
                    EventDate = transaction.Date,
                    AccountId = transaction.AccountId,
                    SupplierId = transaction.SupplierId.ToString(),
                    EventDescription = transaction.Description,
                    EventAmount = child.IsActive ? child.Amount : 0,
                    Type = TransactionType.Income
                };

                if (child.ParentId > 0)
                {
                    newParent.EventId = child.ParentId;
                }

                var newParentVm = new TransactionViewModel(newParent);
                Transactions.Add(newParentVm);

                child.ParentId = newParentVm.Id;
                newParentVm.IsActive = child.IsActive || transaction.IsActive;
            }

            var parent = Transactions.FirstOrDefault(t => t.SupplierId == transaction.SupplierId && t.ParentId == 0);
            if (parent != null)
            {
                transaction.ParentId = parent.Id;
                parent.IsActive = parent.IsActive || transaction.IsActive;

                if (transaction.IsActive)
                {
                    parent.Amount += transaction.Amount;
                }
            }
        }
        
        private void HandleTransactionActivityChange(TransactionViewModel transaction)
        {
            transaction.WhenTransactionActivityChanged.Subscribe((t) =>
            {
                var category = GetCategoryById(transaction.Category);
                if (category == null)
                {
                    return;
                }

                if (transaction.IsActive)
                {
                    category.Spent += transaction.Amount;
                }
                else
                {
                    category.Spent -= transaction.Amount;
                }

                TransactionsView.Refresh();
            });
        }

        private bool TransactionsFilter(object item)
        {
            TransactionViewModel transaction = (TransactionViewModel)item;
            return transaction != null && transaction.IsActive && 
                   SelectedCategory?.Id == transaction.Category;
        }

        private BudgetCategoryViewModel GetCategoryById(Guid categoryId)
        {
            return Categories.FirstOrDefault(category => category.Id.Equals(categoryId));
        }

        private void OnShowNewCategoryExecuted()
        {
            ShowNewCategoryExecuted?.Invoke(null, EventArgs.Empty);
        }
    }
}
