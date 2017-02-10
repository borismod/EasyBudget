using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using DataProvider.Interfaces;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp
{
    public class Auditor
    {
        private FinanceState _book;

        private event EventHandler<StateEventArgs> StateUpdated;
        public IObservable<FinanceState> WhenStateUpdated
        {
            get
            {
                return Observable.FromEventPattern<StateEventArgs>(
                        h => StateUpdated += h,
                        h => StateUpdated -= h)
                    .Select(x => x.EventArgs.State);
            }
        }

        private Auditor()
        {
            var now = DateTime.Now;
            _book = CreateNewBook(now.Year, now.Month);
        }

        public Auditor(DataLoader dataLoader, DateStateViewModel dateStateVm) : this()
        {
            HandleNewTransaction(dataLoader);
            HandleTimeNavigation(dateStateVm);
        }

        private void HandleTimeNavigation(DateStateViewModel dateStateVm)
        {
            dateStateVm.WhenDateStateChanged.Subscribe((state) =>
            {
                _book = CreateNewBook(state.Year, state.Month);
            });
        }

        private void HandleNewTransaction(DataLoader dataLoader)
        {
            dataLoader.WhenTransactionLoaded.Subscribe((transaction) =>
            {
                AuditTransaction(transaction);
                OnStateUpdated(new StateEventArgs(_book));
            });
        }

        private void AuditTransaction(TransactionViewModel transaction)
        {
            HandleTransactionActivityChange(transaction);

            if (!transaction.IsActive)
            {
                return;
            }

            AddValue(transaction);
        }

        private void AddValue(TransactionViewModel transaction)
        {
            if (transaction.Type == TransactionType.Income)
            {
                _book.MonthIncome += Math.Abs(transaction.Amount);
            }
            else
            {
                _book.MonthExpenses += Math.Abs(transaction.Amount);
            }

            _book.MonthBalance = _book.MonthIncome - _book.MonthExpenses;
        }

        private void RemoveValue(TransactionViewModel transaction)
        {
            if (transaction.Type == TransactionType.Income)
            {
                _book.MonthIncome -= Math.Abs(transaction.Amount);
            }
            else
            {
                _book.MonthExpenses -= Math.Abs(transaction.Amount);
            }

            _book.MonthBalance = _book.MonthIncome - _book.MonthExpenses;
        }

        private void HandleTransactionActivityChange(TransactionViewModel transaction)
        {
            transaction.WhenTransactionActivityChanged.Subscribe((t) =>
            {
                if (t.IsActive)
                {
                    AddValue(transaction);
                }
                else
                {
                    RemoveValue(transaction);
                }

                OnStateUpdated(new StateEventArgs(_book));
            });
        }

        private void OnStateUpdated(StateEventArgs e)
        {
            StateUpdated?.Invoke(null, e);
        }

        private FinanceState CreateNewBook(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            return new FinanceState(firstDayOfMonth);
        }

        private class StateEventArgs : EventArgs
        {
            internal FinanceState State { get; }

            internal StateEventArgs(FinanceState state)
            {
                State = state;
            }
        }
    }
}
