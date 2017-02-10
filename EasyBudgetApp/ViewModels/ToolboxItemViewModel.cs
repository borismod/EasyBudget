using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EasyBudgetApp.ViewModels
{
  public class ToolboxItemViewModel : BaseViewModel
  {
    private string _name;
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
        OnPropertyChanged(() => Name);
      }
    }

    private Visual _image;
    public Visual Image
    {
      get
      {
        return _image;
      }
      set
      {
        _image = value;
        OnPropertyChanged(() => Image);
      }
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        _isSelected = value;
        OnPropertyChanged(()=>IsSelected);

        if (value)
        {
          _mainViewModel.CurrentContent = _contentViewModel;
        }
      }
    }

    private readonly BaseViewModel _contentViewModel;
    private readonly MainViewModel _mainViewModel;
    
    public ToolboxItemViewModel(string name, Visual image, BaseViewModel contentViewModel, MainViewModel mainViewModel, bool isSelected = false)
    {
      Name = name;
      Image = image;
      
      _contentViewModel = contentViewModel;
      _mainViewModel = mainViewModel;

      IsSelected = isSelected;
    }
  }
}
