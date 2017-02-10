using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp.Views.Converters
{
  public class BankAccountToVisibileConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is AccountType))
      {
        return false;
      }

      var type = (AccountType)value;
      switch (type)
      {
        case AccountType.Bank:
          return true;
        case AccountType.Credit:
          return false;
        default: break;
      }

      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
