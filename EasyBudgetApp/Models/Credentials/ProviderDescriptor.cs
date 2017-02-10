using System;
using System.Collections.Generic;
using DataProvider;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Models.Credentials
{
  public class ProviderDescriptor : IProviderDescriptor
  {
    public String Name { get; }
    public IDictionary<string, string> Credentials { get; }
    public IList<IAccount> Accounts { get; }

    public ProviderDescriptor(string name, IDictionary<string, string> details, List<IAccount> accounts)
    {
      Name = name;
      Credentials = new Dictionary<string, string>(details);
      Accounts = accounts;
    }
  }
}
