using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataProvider;
using DataProvider.Hapoalim;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Filters
{
  public class FilterRepository
  {
    public static Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>> NetExpenseStrategyHapoalim()
    {
      return new Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>>(
        input => input.Key == VendorId.Hapoalim,
        input =>
        {
          var result = new List<ITransaction>();
          foreach (var transaction in input.Value)
          {
            if (transaction.Type == TransactionType.Expense &&
              !transaction.SupplierId.StartsWith(ServiceProviders.PoalimExpress) &&
              !transaction.SupplierId.StartsWith(ServiceProviders.Visa))
            {
              result.Add(transaction);
            }

          }
          return result;
        });
    }

    public static Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>> ExpenseStrategyHapoalim()
    {
      return new Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>>(
        input => input.Key == VendorId.Hapoalim,
        input =>
        {
          var result = new List<ITransaction>();
          foreach (var transaction in input.Value)
          {
            if (transaction.Type == TransactionType.Expense)
            {
              result.Add(transaction);
            }

          }
          return result;
        });
    }
    
    public static Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>> IncomeStrategyHapoalim()
    {
      return new Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>>(
        input => input.Key == VendorId.Hapoalim,
        input =>
        {
          var result = new List<ITransaction>();
          foreach (var transaction in input.Value)
          {
            if (transaction.Type == TransactionType.Income)
            {
              result.Add(transaction);
            }

          }
          return result;
        });
    }
    
    public static Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>> CacheStrategyAmex()
    {
      return new Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>>(
        input => input.Key == VendorId.Amex,
        input =>
        {
          var result = new List<ITransaction>();
          foreach (var transaction in input.Value)
          {
            if (transaction.SupplierId == ServiceProviders.Amex)
            {
              result.Add(transaction);
            }

          }
          return result;
        });
    }
    
    public static Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>> CacheFreeStrategyAmex()
    {
      return new Strategy<KeyValuePair<VendorId, IEnumerable<ITransaction>>, IEnumerable<ITransaction>>(
        input => input.Key == VendorId.Amex,
        input =>
        {
          var result = new List<ITransaction>();
          foreach (var transaction in input.Value)
          {
            if (transaction.SupplierId != ServiceProviders.Amex)
            {
              result.Add(transaction);
            }
          }
          return result;
        });
    }
  }
}
