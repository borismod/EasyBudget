using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyBudgetApp.Views.Converters
{
  public class IntMonthYearToTextConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2)
      {
        throw new ArgumentException("Incorrect number of arguments");
      }

      if (!(values[0] is int && values[1] is int))
      {
        return string.Empty;
      }

      var month = (int)values[0];
      var year = (int)values[1];

      return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
