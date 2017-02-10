using System;
using System.Collections.Generic;

namespace DataProvider.Amex.Responses
{
  public class DealsListResponse
  {
    public String Moed { get; set; }
    public String PayDay { get; set; }
    public Object DisplayProperties { get; set; }
    public String IsShowDealsInboundForCharge { get; set; }
    public Object IsTooManyRecords { get; set; }
    public String IsShowDealsInboundForInfo { get; set; }
    public String SelectedDateIndex { get; set; }
    public Object StartDate { get; set; }
    public Object EndDate { get; set; }
    public IList<String> DateList { get; set; }
    public Object PaymentSum { get; set; }
    public Object PaymentPercent { get; set; }
    public IList<String> CurrentDate { get; set; }
    public Object IsCashBack { get; set; }
    public Object SelectedCardValid { get; set; }
    public IList<String> CardNumberList { get; set; }
    public Object SelectedCardInfo { get; set; }
    public Object UserId { get; set; }
    public String CardNumberTail { get; set; }
    public String SelectedCardIndex { get; set; }
    public IList<DealResponse> Table2 { get; set; }
    public String IsThereData { get; set; }
    public Object Stage { get; set; }
    public Object ReturnCode { get; set; }
    public Object Message { get; set; }
    public String TablePageNum { get; set; }
    public String IsError { get; set; }
    public String IsCaptcha { get; set; }
    public String IsButton { get; set; }
    public Object SiteName { get; set; }
  }

  public class DealResponse
  {
    public Object DealsInbound { get; set; }
    public Object SupplierId { get; set; }
    public Object SupplierName { get; set; }
    public Object DealSumType { get; set; }
    public Object PaymentSumSign { get; set; }
    public Object PurchaseDate { get; set; }
    public Object FullPurchaseDate { get; set; }
    public Object MoreInfo { get; set; }
    public Object HoraatKeva { get; set; }
    public Object VoucherNumber { get; set; }
    public Object VoucherNumberRatz { get; set; }
    public Object Solek { get; set; }
    public Object PurchaseDateOutbound { get; set; }
    public Object FullPurchaseDateOutbound { get; set; }
    public Object CurrencyId { get; set; }
    public Object CurrentPaymentCurrency { get; set; }
    public Object City { get; set; }
    public Object SupplierNameOutbound { get; set; }
    public Object FullSupplierNameOutbound { get; set; }
    public Object PaymentDate { get; set; }
    public Object FullPaymentDate { get; set; }
    public Object IsShowDealsOutbound { get; set; }
    public Object Adendum { get; set; }
    public Object VoucherNumberRatzOutbound { get; set; }
    public Object IsShowLinkForSupplierDetails { get; set; }
    public Object DealSum { get; set; }
    public Object PaymentSum { get; set; }
    public Object FullSupplierNameHeb { get; set; }
    public Object DealSumOutbound { get; set; }
    public Object PaymentSumOutbound { get; set; }
    public Object IsHoraatKeva { get; set; }
    public Object Stage { get; set; }
    public Object ReturnCode { get; set; }
    public Object Message { get; set; }
    public Object DisplayProperties { get; set; }
    public Object TablePageNum { get; set; }
    public Object IsError { get; set; }
    public Object IsCaptcha { get; set; }
    public Object IsButton { get; set; }
    public Object SiteName { get; set; }
  }
}
