using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DataProvider;
using EasyBudgetApp.Models;

namespace EasyBudgetApp.ViewModels
{
  public class NavigationViewModel : BaseViewModel
  {
    public ObservableCollection<AccountViewModel> Accounts { get; private set; }
    public ObservableCollection<AccountViewModel> Expenses { get; private set; }
    public ObservableCollection<AccountViewModel> Incomes { get; private set; }
    
    private AccountViewModel _selectedAccount;
    private BudgetViewModel _selectedBudget;
    private Double _totalExpense;
    private Double _totalIncome;

    public AccountViewModel SelectedAccount
    {
      get
      {
        return _selectedAccount;
      }
      set
      {
        _selectedAccount = value;
        OnPropertyChanged("SelectedAccount");
      }
    }

    public BudgetViewModel SelectedBudget
    {
      get
      {
        return _selectedBudget;
      }
      set
      {
        _selectedBudget = value;
        OnPropertyChanged("SelectedBudget");
      }
    }

    public Double TotalExpense
    {
      get
      {
        return _totalExpense;
      }
      set
      {
        _totalExpense = value;
        OnPropertyChanged("TotalExpense");
      }
    }

    public Double TotalIncome
    {
      get
      {
        return _totalIncome;
      }
      set
      {
        _totalIncome = value;
        OnPropertyChanged("TotalIncome");
      }
    }

    public ObservableCollection<BudgetViewModel> Budgets { get; private set; }

    public IList<String> ReportTypes { get; private set; }

    public NavigationViewModel(IEnumerable<AccountViewModel> accounts, IEnumerable<AccountViewModel> expenses, IEnumerable<AccountViewModel> incomes)
    {
      Accounts = new ObservableCollection<AccountViewModel>(accounts);
      Expenses = new ObservableCollection<AccountViewModel>(expenses);
      Incomes = new ObservableCollection<AccountViewModel>(incomes);
      Budgets = new ObservableCollection<BudgetViewModel>();

      foreach (var account in accounts)
      {
        account.GroupType = AccountGroupType.Accounts;
        account.NavigationViewModel = this;
      }

      foreach (var expense in expenses)
      {
        expense.GroupType = AccountGroupType.Expenses;
        expense.NavigationViewModel = this;
        TotalExpense += expense.Balance;
      }
      var expenseAccumulator = CreateExpensesAccumulator(expenses);
      Expenses.Add(expenseAccumulator);

      foreach (var income in incomes)
      {
        income.GroupType = AccountGroupType.Incomes;
        income.NavigationViewModel = this;
        TotalIncome += income.Balance;
      }
      var incomeAccumulator = CreateIncomeAccumulator(incomes);
      Incomes.Add(incomeAccumulator);

      Budgets.Add(new BudgetViewModel(this));

      ReportTypes = new[] { "Month Balance" };
    }

    private AccountViewModel CreateIncomeAccumulator(IEnumerable<AccountViewModel> incomes)
    {
      var allTransactions = new List<ITransaction>();
      foreach (var expense in incomes)
      {
        foreach (var transaction in expense.Transactions)
        {
          
          allTransactions.Add(new TransactionDetails
          {
            CurrentBalance = transaction.Balance,
            EventAmount = transaction.Amount,
            EventDate = transaction.Date,
            EventDescription = transaction.Description,
            Type = transaction.Type
          });
        }
      }

      var accumulator = new AccountViewModel(new WalletAccount
      {
        AccountId = 0,
        Balance = Double.NaN,
        Description = "",
        Label = "All accounts",
        VendorId = VendorId.None
      }, allTransactions, AccountType.Custom, false);

      accumulator.GroupType = AccountGroupType.Incomes;
      accumulator.NavigationViewModel = this;
      return accumulator;
    }

    private AccountViewModel CreateExpensesAccumulator(IEnumerable<AccountViewModel> expenses)
    {
      var allTransactions = new List<ITransaction>();
      foreach (var expense in expenses)
      {
        foreach (var transaction in expense.Transactions)
        {
          allTransactions.Add(new TransactionDetails
          {
            CurrentBalance = transaction.Balance,
            EventAmount = transaction.Amount,
            EventDate = transaction.Date,
            EventDescription = transaction.Description,
            Type = transaction.Type
          });
        }
      }

      var accumulator = new AccountViewModel(new WalletAccount()
      {
        AccountId = 0,
        Balance = Double.NaN,
        Description = "",
        Label = "All accounts",
        VendorId = VendorId.None
      }, allTransactions, AccountType.Custom, false);

      accumulator.GroupType = AccountGroupType.Expenses;
      accumulator.NavigationViewModel = this;
      return accumulator;
    }
  }
}
