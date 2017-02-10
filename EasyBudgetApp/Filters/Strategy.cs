using System;

namespace EasyBudgetApp.Filters
{
  public class Strategy<T, V>
  {
    public Predicate<T> Condition { get; private set; }
    public Func<T, V> Result { get; private set; }

    public Strategy(Predicate<T> condition, Func<T, V> result)
    {
      Condition = condition;
      Result = result;
    }

    public V Evaluate(T input)
    {
      if (Condition(input))
      {
        return Result(input);
      }

      return default(V);
    }
  }
}
