using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace EasyBudgetApp.ViewModels.Design
{
  public class ToolbarDesignViewModel
  {
    public ObservableCollection<ToolboxItemViewModel> Items;

    public ToolbarDesignViewModel()
    {
      Items = new ObservableCollection<ToolboxItemViewModel>();
      //Items.Add(new ToolboxItemViewModel("asdasd", new Canvas()));
      //Items.Add(new ToolboxItemViewModel("asdasd", new Canvas()));
      //Items.Add(new ToolboxItemViewModel("asdasd", new Canvas()));
      //Items.Add(new ToolboxItemViewModel("asdasd", new Canvas()));
    }
  }
}
