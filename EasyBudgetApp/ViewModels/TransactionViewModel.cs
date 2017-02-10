using System;
using System.Reactive.Linq;
using DataProvider.Interfaces;

namespace EasyBudgetApp.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        public long AccountId { get; set; }
        public DateTime Date { get; set; }
        public double Balance { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public long SupplierId { get; set; }
        public TransactionType Type { get; set; }
        public long Id { get; set; }
        public long ParentId { get; set; }

        public bool IsActive
        {
            get
            {
                return InactiveTransactionsManager.IsActive(Id);
            }
            set
            {
                if (InactiveTransactionsManager.SetActivity(Id, value) != value)
                {
                    OnTransactionActivityChanged();
                    OnPropertyChanged(() => IsActive);
                }
            }
        }

        private Guid _category;
        public Guid Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged(() => Category);
            }
        }
        
        private event EventHandler<TransactionEventArgs> TransactionActivityChanged;
        public IObservable<TransactionViewModel> WhenTransactionActivityChanged
        {
            get
            {
                return Observable.FromEventPattern<TransactionEventArgs>(
                  h => TransactionActivityChanged += h,
                  h => TransactionActivityChanged -= h)
                  .Select(x => x.EventArgs.Transaction);
            }
        }

        public TransactionViewModel(ITransaction transaction)
        {
            Id = transaction.EventId; 
            AccountId = transaction.AccountId;
            Date = transaction.EventDate;
            Description = transaction.EventDescription;
            Amount = transaction.EventAmount;
            Balance = transaction.CurrentBalance;
            Type = transaction.Type;
            SupplierId = Convert.ToInt64(transaction.SupplierId);
            IsActive = InactiveTransactionsManager.IsActive(Id);

            ParentId = 0;
            
            Category = CategoriesViewModel.GetCategoryBySupplier(SupplierId);

            CategoriesViewModel.WhenCategoryUpdated.Subscribe((changedCategory) =>
            {
                if (SupplierId == changedCategory.Supplier)
                {
                    Category = changedCategory.Category.Id;
                }
            });
        }

        private void OnTransactionActivityChanged()
        {
            EventHandler<TransactionEventArgs> eventHandler = TransactionActivityChanged;
            eventHandler?.Invoke(this, new TransactionEventArgs(this));
        }

        private class TransactionEventArgs : EventArgs
        {
            internal TransactionViewModel Transaction { get; }

            internal TransactionEventArgs(TransactionViewModel transaction)
            {
                Transaction = transaction;
            }
        }
    }
}
