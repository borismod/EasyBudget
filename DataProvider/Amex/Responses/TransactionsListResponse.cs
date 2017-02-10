using System;
using System.Collections.Generic;
using System.Web.UI;
using Newtonsoft.Json;

namespace DataProvider.Amex.Responses
{
    public class TransactionsListResponse
    {
        public HeaderResponse Header { get; set; }
        public TransactionsDetailsResponse CardsTransactionsListBean { get; set; }
    }

    public class TransactionsDetailsResponse
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public string CardIdx { get; set; }
        public string Moed { get; set; }
        public string PayDay { get; set; }
        public string SelectedDateIndex { get; set; }
        public IList<string> DateList { get; set; }
        public string TotalChargeNis { get; set; }
        public string TotalChargeDollar { get; set; }
        public string TotalChargeEuro { get; set; }
        public string IsShowDealsInboundForCharge { get; set; }
        public string IsTooManyRecords { get; set; }
        public string IsShowDealsInboundForInfo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaymentSum { get; set; }
        public string PaymentPercent { get; set; }
        public IndexResponse Index { get; set; }
        public string IsCashBack { get; set; }
        public string CurrentDate { get; set; }
        public IList<string> CardNumberList { get; set; }
        public string SelectedCardInfo { get; set; }
        public string UserId { get; set; }
        public string CardNumberTail { get; set; }
        public string Card6Digits { get; set; }
        public int SelectedCardIndex { get; set; }
        public object Id { get; set; }
        public object Card { get; set; }
        public string IsThereData { get; set; }
        public string Stage { get; set; }
        public string ReturnCode { get; set; }
        public string Message { get; set; }
        public string ReturnMessage { get; set; }
        public string DisplayProperties { get; set; }
        public int TablePageNum { get; set; }
        public bool IsError { get; set; }
        public bool IsCaptcha { get; set; }
        public bool IsButton { get; set; }
        public string SiteName { get; set; }
    }

    public class IndexResponse
    {
        public string AllCards { get; set; }
        public IList<CardTransactionsResponse> CurrentCardTransactions { get; set; }
    }

    public class CardTransactionsResponse
    {
        public string CardTransactions { get; set; }

        [JsonProperty(PropertyName = "txnIsrael")]
        public IList<CardTransaction> TxnIsrael { get; set; }

        [JsonProperty(PropertyName = "txnInfo")]
        public IList<CardTransaction> TxnInfo { get; set; }

        [JsonProperty(PropertyName = "txnAbroad")]
        public IList<CardTransaction> TxnAbroad { get; set; }
    }
    
    public class CardTransaction
    {
        public object Adendum { get; set; }
        public string City { get; set; }
        public string CurrencyId { get; set; }
        public string CurrentPaymentCurrency { get; set; }
        public string DealsInbound{ get; set; }
        public string DealSum { get; set; }
        public string DealSumOutbound { get; set; }
        public string DealSumType { get; set; }
        public string DisplayProperties{ get; set; }
        public string FullPaymentDate { get; set; }
        public string FullPurchaseDate { get; set; }
        public string FullPurchaseDateOutbound { get; set; }
        public string FullSupplierNameHeb { get; set; }
        public string FullSupplierNameOutbound { get; set; }
        public string HoraatKeva { get; set; }
        public string IsButton { get; set; }
        public string IsCaptcha { get; set; }
        public string IsError { get; set; }
        public string IsHoraatKeva { get; set; }
        public string IsShowDealsOutbound { get; set; }
        public string IsShowLinkForSupplierDetails { get; set; }
        public string Message { get; set; }
        public string MoreInfo { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentSum { get; set; }
        public string PaymentSumOutbound { get; set; }
        public string PaymentSumSign { get; set; }
        public string PurchaseDate { get; set; }
        public string PurchaseDateOutbound { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public string SiteName { get; set; }
        public string Solek { get; set; }
        public string Stage { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierNameOutbound { get; set; }
        public string TablePageNum { get; set; }
        public string VoucherNumber { get; set; }
        public string VoucherNumberRatz { get; set; }
        public string VoucherNumberRatzOutbound { get; set; }
    }
}


