using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DataProvider.Interfaces;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
    public class IncomeViewModel : AccountsBaseViewModel
    {
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
        public ICollectionView TransactionsView { get; }

        public IncomeViewModel(DataLoader dataLoader, Auditor auditor, DateStateViewModel dateState) : base(dataLoader, auditor, dateState)
        {
            Transactions = new ObservableCollection<TransactionViewModel>();

            HandleIncomeTransactions(dataLoader);
            HandleAccountsLoading();

            TransactionsView = CollectionViewSource.GetDefaultView(Transactions);
            TransactionsView.Filter = TransactionsFilter;
            TransactionsView.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
        }

        private void HandleAccountsLoading()
        {
            AccountViewModel.WhenShowNewAccountExecuted.Subscribe((account) =>
            {
                TransactionsView.Refresh();
            });
        }

        private void HandleIncomeTransactions(DataLoader dataLoader)
        {
            dataLoader.WhenTransactionLoaded.Subscribe((transaction) =>
            {
                if (transaction.Type == TransactionType.Income)
                {
                    HandleAgregatedTransaction(transaction);
                    Transactions.Add(transaction);
                    HandleTransactionActivityChange(transaction);
                    TransactionsView.Refresh();
                }
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
                SetParentActivity(transaction, transaction.IsActive);
                TransactionsView.Refresh();
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

        private void RemoveById(long id)
        {
            var viewModelToRemove = _transactions.FirstOrDefault(t => t.Id == id);
            if (viewModelToRemove != null)
            {
                var parentId = viewModelToRemove.ParentId;
                Transactions.Remove(viewModelToRemove);

                if (Transactions.All(t => t.ParentId != parentId))
                {
                    RemoveById(parentId);
                }
            }
        }

        private bool TransactionsFilter(object item)
        {
            TransactionViewModel transaction = (TransactionViewModel)item;
            AccountViewModel account;

            if (transaction == null || !transaction.IsActive)
            {
                return false;
            }

            AccountsById.TryGetValue(transaction.AccountId, out account);
            return account != null && account.IsEnabled;
        }

        protected override void HandleDateStateChange(DateStateViewModel dateState)
        {
            base.HandleDateStateChange(dateState);
            dateState.WhenDateStateChanged.Subscribe((state) =>
            {
                Transactions.Clear();
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
