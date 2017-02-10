using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DataProvider;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
  public class AccountViewModel : BaseViewModel
  {
    internal NavigationViewModel NavigationViewModel { private get; set; }
    public AccountType Type { get; private set; }
    public AccountGroupType GroupType { get; set; }
    public string Title { get; private set; }
    public string SubTitle { get; private set; }
    public Double Balance { get; private set; }
    
    public ObservableCollection<TransactionViewModel> Transactions { get; private set; }

    private bool _isSelected;
    public bool IsSelected {
      get
      {
        return _isSelected;
      }
      set
      {
        _isSelected = value;
        OnPropertyChanged("IsSelected");

        if (value == false && NavigationViewModel.SelectedAccount == this)
        {
          NavigationViewModel.SelectedAccount = null;
        }
        else if (value == true)
        {
          NavigationViewModel.SelectedBudget = null;
          NavigationViewModel.SelectedAccount = this;
        }
      }
    }

    public AccountViewModel()
    {
      Transactions = new ObservableCollection<TransactionViewModel>();
    }

    public AccountViewModel(IBankAccount model, IEnumerable<ITransaction> transactions, bool calculateBalance) : this()
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }
      if (transactions == null)
      {
        throw new ArgumentNullException("transactions");
      }

      Title = model.BankName;
      SubTitle = string.Format("{0}-{1}", model.BranchNumber, model.AccountNumber);
      Type = AccountType.Bank;
      Transactions = new ObservableCollection<TransactionViewModel>();

      foreach (var transaction in transactions)
      {
        Transactions.Add(new TransactionViewModel(transaction));
      }

      Balance = calculateBalance ? CalculateBalance(transactions): model.Balance;  
    }

    public AccountViewModel(ICreditCardAccount model, IEnumerable<ITransaction> transactions, bool calculateBalance)
      : this()
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }
      if (transactions == null)
      {
        throw new ArgumentNullException("transactions");
      }

      Title = model.Label;
      SubTitle =  (String.IsNullOrWhiteSpace(model.PartnerName) || String.IsNullOrWhiteSpace(model.PartnerName)) 
        ? model.CardNumber.ToString() 
        : string.Format("{0}-{1}", model.PartnerName, model.CardNumber);
      Type = AccountType.CreditCard;
      Transactions = new ObservableCollection<TransactionViewModel>();
      foreach (var transaction in transactions)
      {
        Transactions.Add(new TransactionViewModel(transaction));
      }

      Balance = calculateBalance ? CalculateBalance(transactions) : model.Balance;  
    }

    public AccountViewModel(ICustomAccount model, IEnumerable<ITransaction> transactions, AccountType type, bool calculateBalance)
      : this()
    {
      if (model == null)
      {
        throw new ArgumentNullException("model");
      }
      if (transactions == null)
      {
        throw new ArgumentNullException("transactions");
      }

      Title = model.Label;
      SubTitle = model.Description;
      Type = type;
      Transactions = new ObservableCollection<TransactionViewModel>();
      foreach (var transaction in transactions)
      {
        Transactions.Add(new TransactionViewModel(transaction));
      }

      Balance = calculateBalance ? CalculateBalance(transactions) : model.Balance;  
    }

    private Double CalculateBalance(IEnumerable<ITransaction> transactions)
    {
      Double balance = transactions.Sum(transaction => transaction.EventAmount);
      return Math.Round(balance, 2);
    }
  }

  public enum AccountGroupType
  {
    Accounts, Expenses, Incomes, Budget
  }
}
