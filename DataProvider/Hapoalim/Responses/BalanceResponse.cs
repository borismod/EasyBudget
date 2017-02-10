using System;

namespace DataProvider.Hapoalim.Responses
{
  public class BalanceResponse
  {
    public Object Methadata { get; set; }
    public Double CurrentAccountLimitsAmount { get; set; }
    public Double WithdrawalBalance { get; set; }
    public Double CurrentBalance { get; set; }
    public Double CreditLimitUtilizationPercent { get; set; }
    public int CreditLimitUtilizationExistanceCode { get; set; }
    public Object CreditLimitDescription { get; set; }
    public Double CreditLimitAmount { get; set; }
    public String FormattedCurrentAccountLimitsAmount { get; set; }
    public String FormattedCurrentBalance { get; set; }
    public String FormattedWithdrawalBalance { get; set; }
    public String FormattedCurrentDate { get; set; }
  }
}

// Example:

//{
//    "metadata":
//    {
//        "links":{}
//    },
//    "currentAccountLimitsAmount":0.0,
//    "withdrawalBalance":1400.05,
//    "currentBalance":1400.05,
//    "creditLimitUtilizationPercent":0.0,
//    "creditLimitUtilizationExistanceCode":2,
//    "creditLimitDescription":null,
//    "creditLimitAmount":0.0,
//    "creditLimitUtilizationAmount":0.0,
//    "formattedCurrentAccountLimitsAmount":"₪ 0.00",
//    "formattedCurrentBalance":"₪ 1,400.05",
//    "formattedWithdrawalBalance":"₪ 1,400.05",
//    "formattedCurrentDate":"05/01"
//}
