using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EasyBudgetApp.Views.Converters
{
  public class DoubleToBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Double))
      {
        return new SolidColorBrush(Colors.Black);
      }

      var balance = (Double) value;
      if (Double.IsNaN(balance))
      {
        return new SolidColorBrush(Colors.Transparent);
      }

      return balance >= 0 ? new SolidColorBrush(Colors.DarkGreen) : new SolidColorBrush(Colors.DarkRed);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
