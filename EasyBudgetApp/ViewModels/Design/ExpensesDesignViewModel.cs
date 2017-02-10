using System;
using System.Collections.Generic;
using DataProvider.Interfaces;
using DataProvider.Model;

namespace EasyBudgetApp.ViewModels.Design
{
    public class ExpensesDesignViewModel
    {
        public static List<TransactionViewModel> TransactionsView
        {
            get
            {
                var t1 = new TransactionViewModel(new BankTransaction
                {
                    AccountId = 123123,
                    CurrentBalance = 123.32,
                    EventAmount = 10,
                    EventDate = DateTime.Now,
                    Type = TransactionType.Expense,
                    EventDescription = "This is transaction description",
                    EventId = 22233,
                    SupplierId = "1"
                });

                var t2 = new TransactionViewModel(new BankTransaction
                {
                    AccountId = 123123,
                    CurrentBalance = 123.32,
                    EventAmount = 8,
                    EventDate = DateTime.Now,
                    Type = TransactionType.Expense,
                    EventDescription = "This is another transaction description",
                    EventId = 24321,
                    SupplierId = "1"
                });

                var t3 = new TransactionViewModel(new BankTransaction
                {
                    AccountId = 123123,
                    CurrentBalance = 123.32,
                    EventAmount = 34,
                    EventDate = DateTime.Now,
                    Type = TransactionType.Expense,
                    EventDescription = "This is another transaction description 2",
                    EventId = 1233222,
                    SupplierId = "2"
                });

                t3.ParentId = t1.Id;
            
                return new List<TransactionViewModel>
                {
                    t1,t2,t3
                };
            }
        }
    }
}
