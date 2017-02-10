using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBudgetApp.ViewModels.Design
{
  public class MainDesignViewModel
  {
    public ToolbarDesignViewModel Toolbar { get; set; }

    public MainDesignViewModel()
    {
      Toolbar = new ToolbarDesignViewModel();
    }
  }
}
