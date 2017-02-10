using System;
using System.Collections.ObjectModel;
using DataProvider;
using DataProvider.Interfaces;
using DataProvider.Model;

namespace EasyBudgetApp.ViewModels.Design
{
  public class IncomeDesignViewModel
  {
    public ObservableCollection<TransactionViewModel> Transactions { get; set; }

    public IncomeDesignViewModel()
    {
      Transactions = new ObservableCollection<TransactionViewModel>();
      
      Transactions.Add(new TransactionViewModel(new BankTransaction
      {
        AccountId = 123,
        CurrentBalance = 100.0,
        EventAmount = 30.0,
        EventDate = DateTime.Now,
        EventDescription = "Very interesting event",
        EventId = 123321,
        SupplierId = "123123",
        Type = TransactionType.Income
      }));

      Transactions.Add(new TransactionViewModel(new BankTransaction
      {
        AccountId = 123,
        CurrentBalance = 400.0,
        EventAmount = 320.0,
        EventDate = DateTime.Now,
        EventDescription = "Some event ",
        EventId = 13432,
        SupplierId = "1343452",
        Type = TransactionType.Income
      }));
    }
  }
}
