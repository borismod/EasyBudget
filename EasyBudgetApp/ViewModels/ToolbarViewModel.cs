using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBudgetApp.ViewModels
{
  public class ToolbarViewModel : BaseViewModel
  {
    private ObservableCollection<ToolboxItemViewModel> _items;
    public ObservableCollection<ToolboxItemViewModel> Items
    {
      get
      {
        return _items;
      }

      set
      {
        _items = value;
        OnPropertyChanged(() => Items);
      }
    }

    public ToolbarViewModel()
    {
      Items = new ObservableCollection<ToolboxItemViewModel>();   
    }
  }
}
