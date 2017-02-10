using System;

namespace DataProvider.Hapoalim.Responses
{
  public class TransactionResponse
  {
    public long EventDate { get; set; }
    public string FormattedEventDate { get; set; }
    public int SerialNumber { get; set; }
    public int ActivityTypeCode { get; set; }
    public string ActivityDescription { get; set; }
    public int TextCode { get; set; }
    public long ReferenceNumber { get; set; }
    public long ValueDate { get; set; }
    public string FormattedValueDate { get; set; }
    public Double EventAmount { get; set; }
    public int EventActivityTypeCode { get; set; }
    public Double CurrentBalance { get; set; }
    public int InternalLinkCode { get; set; }
    public long OriginalEventCreateDate { get; set; }
    public string FormattedOriginalEventCreateDate { get; set; }
    public string TransactionType { get; set; }
    public int DataGroupCode { get; set; }
    public BeneficiaryDetails BeneficiaryDetailsData { get; set; }
    public long ExpandedEventDate { get; set; }
    public int ExecutingBranchNumber { get; set; }
    public long EventId { get; set; }
    public string Details { get; set; }
    public string PfmDetails { get; set; }
    public string DifferentDateIndication { get; set; }
    public string RejectedDataEventPertainingIndication { get; set; }

    public class BeneficiaryDetails
    {
      public string PartyName { get; set; }
      public string PartyHeadline { get; set; }
      public string MessageDetail { get; set; }
      public string MessageHeadline { get; set; }
    }
  }
}


//Example

//{
//    "retrievalTransactionData": {
//            "balanceAmountDisplayIndication": null,
//            "branchNumber": 617,
//            "accountNumber": 527482,
//            "retrievalMinDate": 20160103,
//            "retrievalMaxDate": 20160203,
//            "retrievalStartDate": 0,
//            "retrievalEndDate": 0,
//            "eventCounter": 1,
//            "joinPfm": false
//        },
//    "message": [],
//    "transactions": [
//            {
//                "eventDate": 20160110,
//                "formattedEventDate": "1/10/2016 12:00:00 AM",
//                "serialNumber": 0,
//                "activityTypeCode": 515,
//                "activityDescription": "פועלים-משכנתא",
//                "textCode": 803,
//                "referenceNumber": 812529,
//                "referenceCatenatedNumber": 13289,
//                "valueDate": 20160110,
//                "formattedValueDate": "1/10/2016 12:00:00 AM",
//                "eventAmount": 7320.04,
//                "eventActivityTypeCode": 2,
//                "currentBalance": 0,
//                "internalLinkCode": 9,
//                "originalEventCreateDate": 0,
//                "formattedOriginalEventCreateDate": null,
//                "transactionType": "FUTURE",
//                "dataGroupCode": 7,
//                "beneficiaryDetailsData": null,
//                "expandedEventDate": "2016011000000",
//                "executingBranchNumber": 0,
//                "eventId": 0,
//                "details": null,
//                "pfmDetails": null,
//                "differentDateIndication": null,
//                "rejectedDataEventPertainingIndication": "N"
//            }
//        ]
//}