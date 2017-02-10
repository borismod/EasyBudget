using System;

namespace EasyBudgetApp
{
    public class FinanceState
    {
        public double MonthIncome { get; set; }
        public double MonthExpenses { get; set; }
        public double MonthBalance { get; set; }
        public DateTime StateDate{ get; set; }

        public FinanceState(DateTime date)
        {
            MonthExpenses = MonthIncome = MonthBalance = 0.00;
            StateDate = date;
        }
    }
}
