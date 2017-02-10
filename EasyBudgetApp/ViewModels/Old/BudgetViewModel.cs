using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;

namespace EasyBudgetApp.ViewModels
{
  public class BudgetViewModel : BaseViewModel
  {
    internal NavigationViewModel NavigationViewModel { private get; set; }
    public ObservableCollection<CategoryViewModel> Categories { get; set; }
    public String Name { get; set; }

    public AccountGroupType GroupType { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        _isSelected = value;
        OnPropertyChanged("IsSelected");

        if (value == false && NavigationViewModel.SelectedBudget == this)
        {
          NavigationViewModel.SelectedBudget = null;
        }
        else if (value == true)
        {
          NavigationViewModel.SelectedAccount = null;
          NavigationViewModel.SelectedBudget = this;
        }
      }
    }

    public BudgetViewModel(NavigationViewModel navigationViewModel)
    {
      NavigationViewModel = navigationViewModel;
      GroupType = AccountGroupType.Budget;
      Categories = new ObservableCollection<CategoryViewModel>();
      Name = "Home Budget";

      Categories.Add(new CategoryViewModel { Name = "בית" });
      Categories.Add(new CategoryViewModel { Name = "שירותים" });
      Categories.Add(new CategoryViewModel { Name = "אוכל" });
      Categories.Add(new CategoryViewModel { Name = "בגדים" });
      Categories.Add(new CategoryViewModel { Name = "בילוים" });
      Categories.Add(new CategoryViewModel { Name = "רכב" });
      Categories.Add(new CategoryViewModel { Name = "בריאות וביטוחים" });
      Categories.Add(new CategoryViewModel { Name = "שונות" });
    }
  }
}
