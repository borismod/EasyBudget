using System.Collections.Generic;
using System.Linq;

namespace EasyBudgetApp.Filters
{
  public class StrategyService<T, V>
  {
    private readonly List<Strategy<T, V>> _strategies;

    public StrategyService(params Strategy<T, V>[] strategies)
    {
      _strategies = strategies.ToList();
    }

    public V Evaluate(T input)
    {
      var result = default(V);
      foreach (var strategy in _strategies)
      {
        result = strategy.Evaluate(input);

        if (result != null)
        {
          break;
        }
      }

      return result;
    }
  }
}
