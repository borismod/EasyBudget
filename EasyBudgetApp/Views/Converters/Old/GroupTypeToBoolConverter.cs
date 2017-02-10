using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using EasyBudgetApp.ViewModels;

namespace EasyBudgetApp.Views.Converters
{
  public class GroupTypeToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is AccountGroupType))
      {
        return false;
      }

      var val = System.Convert.ToInt16(value);
      var expectedVal = System.Convert.ToInt16(parameter);

      return val == expectedVal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
