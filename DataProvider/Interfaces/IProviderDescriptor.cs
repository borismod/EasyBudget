using System.Collections.Generic;

namespace DataProvider.Interfaces
{

  public interface IProviderDescriptor
  {
    string Name { get; }
    IDictionary<string, string> Credentials { get; }
    IList<IAccount> Accounts { get; }
  }
}
