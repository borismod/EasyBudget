using System;
using System.Globalization;
using System.Windows.Data;

namespace EasyBudgetApp.Views.Converters
{
  public class DoubleToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string result = String.Empty;
      var input = value as string;

      if (!string.IsNullOrEmpty(input) && !input.Equals(Double.NaN.ToString()))
      {
        result = input;
      }

      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
