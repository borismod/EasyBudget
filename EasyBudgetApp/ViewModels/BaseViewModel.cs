using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

namespace EasyBudgetApp.ViewModels
{
  public class BaseViewModel : INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _disposed;
    public virtual string DisplayName { get; protected set; }
    protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

    protected BaseViewModel()
    {
    }

    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(string propertyName)
    {
      if (TypeDescriptor.GetProperties(this)[propertyName] == null)
      {
        string msg = "Invalid property name: " + propertyName;

        if (ThrowOnInvalidPropertyName)
        {
          throw new Exception(msg);
        }

        Debug.Fail(msg);
      }
    }

    protected virtual void OnPropertyChanged<T>(Expression<Func<T>> selectorExpression)
    {
      if (selectorExpression == null)
      {
        throw new ArgumentNullException(nameof(selectorExpression));
      }

      MemberExpression body = selectorExpression.Body as MemberExpression;
      if (body == null)
      {
        throw new ArgumentException("The body must be a member expression");
      }

      OnPropertyChanged(body.Member.Name);
    }
    protected virtual void OnPropertyChanged(string propertyName)
    {
      VerifyPropertyName(propertyName);

      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler == null)
      {
        return;
      }

      var e = new PropertyChangedEventArgs(propertyName);
      handler(this, e);
    }


    public void Dispose()
    {
      Dispose(true);
    }
    protected virtual void Dispose(bool disposing)
    {
      if (_disposed)
      {
        return;
      }

      _disposed = true;
    }
  }
}
