using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider;
using EasyBudgetApp.Models.Credentials;

namespace EasyBudgetApp.ViewModels
{
  public class CompanyViewModel : BaseViewModel
  {
    public string _name;
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

    public bool _isSupported;
    public bool IsSupported
    {
      get
      {
        return _isSupported;
      }
      set
      {
        _isSupported = value;
        OnPropertyChanged(() => IsSupported);
      }
    }

    ObservableCollection<NameValueViewModel> _credentialFields;

    public ObservableCollection<NameValueViewModel> CredentialFields
    {
      get
      {
        return _credentialFields;
      }
      set
      {
        _credentialFields = value;
        OnPropertyChanged(() => CredentialFields);
      }
    }

    public static CompanyViewModel Empty => new CompanyViewModel();

    public CompanyViewModel(CompanyProfile descriptor)
    {
      Name = descriptor.Name;
      IsSupported = descriptor.Credentials.Any();

      CredentialFields = new ObservableCollection<NameValueViewModel>();
      foreach (var field in descriptor.Credentials)
      {
        CredentialFields.Add(new NameValueViewModel(field, String.Empty));
      }
    }

    private CompanyViewModel()
    {
      Name = string.Empty;
      IsSupported = false;
      CredentialFields = new ObservableCollection<NameValueViewModel>();
    }

    public class NameValueViewModel : BaseViewModel 
    {
      public string _name;
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

      public string _value;
      public string Value
      {
        get
        {
          return _value;
        }
        set
        {
          _value = value;
          OnPropertyChanged(() => Value);
        }
      }

      internal NameValueViewModel(string name, string value)
      {
        Name = name;
        Value = value;
      }
    }
  }
}
