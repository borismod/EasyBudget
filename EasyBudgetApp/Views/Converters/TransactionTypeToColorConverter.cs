using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using DataProvider;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Views.Converters
{
  public class TransactionTypeToColorConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is TransactionType))
      {
        return new SolidColorBrush(Colors.Black);
      }

      var type = (TransactionType)value;
      return type == TransactionType.Income ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.DarkRed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
