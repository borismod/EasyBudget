using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DataProvider.Interfaces;
using DataProvider.Model;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProvider.Amex
{
    public class AmexDataProvider : IDataProvider
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string CompanyName = "American Express";
        private readonly IAmexApi _amexApi;
        private readonly IList<ICreditCardAccount> _cards;
        public bool IsReady => _amexApi.IsReady;
        public string Name => CompanyName;
        public AmexDataProvider(IAmexApi amexApi)
        {
            _amexApi = amexApi;
            _cards = new List<ICreditCardAccount>();
            _log.Debug("AmexDataProvider is created");
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            IList<ICreditCardAccount> result = new List<ICreditCardAccount>();

            var cards = _amexApi.GetCards();
            if (cards.Table1 == null)
            {
                return result;
            }

            foreach (var card in cards.Table1)
            {
                var newCard = new CreditCardAccount
                {
                    VendorId = VendorId.Amex,
                    AccountId = card.CardNumber,
                    CompanyName = CompanyName,
                    Label = CompanyName,
                    BankAccountId = card.BankAccountId,
                    BankBranchId = card.BankBranchId,
                    BankId = card.BankBranchId,
                    CardName = card.CardName,
                    CardNumber = card.CardNumber,
                    PartnerName = card.PartnerName,
                    Balance = Double.NaN
                };

                result.Add(newCard);
            }

            if (_cards.Count == 0 && result.Count > 0)
            {
                foreach (var newCard in result)
                {
                    _cards.Add(newCard);
                }
            }

            return result;
        }

        public IEnumerable<ITransaction> GetTransactions(long accountId, DateTime startTime, DateTime endTime)
        {
            var index = GetCardIndex(accountId);
            var transactions = _amexApi.GetTransactions(index);

            var result = new List<ITransaction>();
            foreach (var transaction in transactions)
            {
                if (transaction.DealsInbound is String && ((String)transaction.DealsInbound).Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                {
                    var purchaseDate = transaction.FullPurchaseDate as string;
                    string[] date = string.IsNullOrEmpty(purchaseDate) ? new[] { "1", "1", "2000" } : purchaseDate.Split('/');
                    var voucherNumberRatz = transaction.VoucherNumberRatz is string ? Convert.ToInt64(transaction.VoucherNumberRatz) : 0;
                    var supplierName = transaction.SupplierName is string ? transaction.SupplierName.ToString() : "Unknown";
                    var paymentSum = transaction.PaymentSum is string ? Convert.ToDouble(transaction.PaymentSum) : 0;
                    var supplierId = transaction.SupplierId is string ? transaction.SupplierId as String : "0";

                    if (voucherNumberRatz != 0)
                    {
                        result.Add(new CreditTransaction
                        {
                            AccountId = accountId,
                            SupplierId = supplierId,
                            EventId = voucherNumberRatz,
                            EventDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0])),
                            EventDescription = supplierName,
                            CurrentBalance = Double.NaN,
                            EventAmount = paymentSum,
                            Type = TransactionType.Outcome,
                        });
                    }
                }
                else
                {
                    var purchaseDate = transaction.FullPurchaseDateOutbound as string;
                    string[] date = string.IsNullOrEmpty(purchaseDate) ? new[] { "1", "1", "2000" } : purchaseDate.Split('/');
                    var voucherNumberRatz = transaction.VoucherNumberRatzOutbound is string ? Convert.ToInt64(transaction.VoucherNumberRatzOutbound) : 0;
                    var supplierName = transaction.SupplierNameOutbound is string ? transaction.SupplierNameOutbound.ToString() : "Unknown";
                    var paymentSum = transaction.PaymentSumOutbound is string ? Convert.ToDouble(transaction.PaymentSumOutbound) : 0;
                    var supplierId = transaction.SupplierId is string ? transaction.SupplierId as String : "0";

                    if (voucherNumberRatz != 0)
                    {
                        result.Add(new CreditTransaction
                        {
                            AccountId = accountId,
                            SupplierId = supplierId,
                            EventId = voucherNumberRatz,
                            EventDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0])),
                            EventDescription = supplierName,
                            CurrentBalance = Double.NaN,
                            EventAmount = paymentSum,
                            Type = TransactionType.Outcome,
                        });
                    }
                }
            }

            return result;
        }

        public IEnumerable<object> GetTransactionDetails(long accountId, string date, long transactionId)
        {
            var cardIndex = GetCardIndex(accountId);
            var details = ((AmexApi)_amexApi).GetTransactionDetails(cardIndex, date, transactionId.ToString());
            return new List<object>();
        }

        private int GetCardIndex(long accountId)
        {
            int index = 0;
            var card = _cards[index];
            while (card.CardNumber != accountId && _cards.Count > index)
            {
                index++;
                card = _cards[index];
            }
            return index;
        }

        public double GetBalance(long accountId)
        {
            throw new NotImplementedException();
        }

        public void RefreshData(long accountId, Func<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _amexApi.Dispose();
        }
    }
}
