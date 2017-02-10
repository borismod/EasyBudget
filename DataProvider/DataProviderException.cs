using System;
using DataProvider.Interfaces;

namespace DataProvider
{
  public class DataProviderException : Exception
  {
    public IProviderDescriptor ProviderDescriptor { get; private set; }
    public override string Message { get; }

    public DataProviderException(IProviderDescriptor descriptor, string message)
    {
      ProviderDescriptor = descriptor;
      Message = message;
    }
  }
}
