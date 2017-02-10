using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace EasyBudgetApp.Views.Converters
{
  [ContentProperty("Converters")]
  [ContentWrapper(typeof(ValueConverterCollection))]
  public class ConverterChain : IValueConverter
  {
    ValueConverterCollection _converters;
    public ValueConverterCollection Converters
    {
      get
      {
        return _converters ?? (_converters = new ValueConverterCollection());
      }
    }

    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (var valueConverter in Converters)
      {
        value = valueConverter.Convert(value, targetType, parameter, culture);
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

  /// <summary>Represents a collection of <see cref="IValueConverter"/>s.</summary>
  public sealed class ValueConverterCollection : Collection<IValueConverter> { }
}
