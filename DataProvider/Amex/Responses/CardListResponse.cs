using System.Collections.Generic;

namespace DataProvider.Amex.Responses
{
    public class CardListResponse
    {
        public HeaderResponse Header { get; set; }

        public CardListDeatils CardsList_102DigitalBean { get; set; }
    }

    public class CardListInfo
    {
        public long BankAccountId { get; set; }
        public int BankBranchId { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int CardIndex { get; set; }
        public string CardName { get; set; }
        public long CardNumber { get; set; }
        public string Club1 { get; set; }
        public object ConnectionType { get; set; }
        public string DisplayProperties { get; set; }
        public string ExparationDate { get; set; }
        public string HolderId { get; set; }
        public bool IsButton { get; set; }
        public bool IsCaptcha { get; set; }
        public bool IsError { get; set; }
        public int CodeShiluah { get; set; }
        public int KvutzatShiluah { get; set; }
        public string LogoBigUrl { get; set; }
        public string LogoSmallUrl { get; set; }
        public string Message { get; set; }
        public string PartnerName { get; set; }
        public int PaymentDate { get; set; }
        public object ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public int ServiceType { get; set; }
        public string SiteName { get; set; }
        public object Stage { get; set; }
        public string StatusDate { get; set; }
        public int TablePageNum { get; set; }
        public string UserName { get; set; }
    }

    public class CardListDeatils
    {
        public string DisplayProperties { get; set; }
        public string Id { get; set; }
        public string Info { get; set; }
        public object InvalidCards { get; set; }
        public bool IsButton { get; set; }
        public bool IsCaptcha { get; set; }
        public bool IsError { get; set; }
        public object IsThereData { get; set; }
        public string Message { get; set; }
        public object ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public object SiteName { get; set; }
        public object Stage { get; set; }
        public IList<CardListInfo> Table1 { get; set; }
        public IList<CardListInfo> Table2 { get; set; }
        public int TablePageNum { get; set; }
        public string ValidCards { get; set; }
    }
}
