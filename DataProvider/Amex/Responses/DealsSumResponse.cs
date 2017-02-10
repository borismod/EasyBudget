using System;
using System.Collections.Generic;

namespace DataProvider.Amex.Responses
{
  public class DealsSumResponse
  {
    private IList<DealsSumInfo> CardsTotal { get; set; }
    public object Stage { get; set; }
    public object ReturnCode { get; set; }
    public object Message { get; set; }
    public object DisplayProperties { get; set; }
    public int TablePageNum { get; set; }
    public bool IsError { get; set; }
    public bool IsCaptcha { get; set; }
    public bool IsButton { get; set; }
    public object SiteName { get; set; }

    public class DealsSumInfo
    {
      public string CardName { get; set; }
      public long CardNumberTail { get; set; }
      public bool IsValid { get; set; }
      public int ServiceType { get; set; }
      public int AccountBank{ get; set; }
      public int Shiuch { get; set; }
      public int PaymentDate { get; set; }
      public object TotalSumList { get; set; }
      public string StatementDate { get; set; }
      public Double TotalDolarCharge { get; set; }
      public Double TotalEuroCharge { get; set; }
      public Double TotalShekelCharge { get; set; }
      public Double TotalNormalShekelCharge { get; set; }
      public Double TotalPaymentsShekelCharge { get; set; }
      public Double TotalKPaymentsShekelCharge { get; set; }
      public Double TotalCreditShekelCharge { get; set; }
      public Double TotalShekelAbroadCharge { get; set; }
      public Double TotalRefundCharge { get; set; }
      public Double TotalDelayedCharge { get; set; }
      public Double FlexibleAmountCharge { get; set; }
      public Double OverloadAmountCharge { get; set; }
      public String OnlyCardName { get; set; }
    }
  }
}
