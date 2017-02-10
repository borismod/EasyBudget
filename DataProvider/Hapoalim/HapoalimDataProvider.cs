using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataProvider.Hapoalim.Responses;
using DataProvider.Interfaces;
using DataProvider.Model;
using log4net;
using Newtonsoft.Json;

namespace DataProvider.Hapoalim
{
    public class HapoalimDataProvider : IDataProvider
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string BankName = "Hapoalim";
        private IHapoalimApi _hapoalimApi;

        private IDictionary<long, AccountResponse> _accountsById;
        private IDictionary<long, TransactionsResponse> _transactions;
        public bool IsReady => _hapoalimApi.IsReady;
        public string Name => BankName;
        public HapoalimDataProvider(IHapoalimApi hapoalimApi)
        {
            _hapoalimApi = hapoalimApi;
            _accountsById = new Dictionary<long, AccountResponse>();
            _transactions = new Dictionary<long, TransactionsResponse>();
            _log.Debug("HapoalimDataProvider is created");
        }

        #region IDataProvider

        public IEnumerable<IAccount> GetAccounts()
        {
            _accountsById.Clear();
            var result = new List<IAccount>();
            var accounts = _hapoalimApi.GetAccountsData();
            if (accounts == null)
            {
                return result;
            }

            foreach (var account in accounts)
            {
                var idAsText = string.Format("{0}{1}{2}", account.BankNumber, account.BranchNumber, account.AccountNumber);
                var id = Convert.ToInt64(idAsText);
                _accountsById.Add(id, account);

                //if (account.AccountClosingReasonCode == 0)
                //{
                //}
            }

            foreach (var accountPair in _accountsById)
            {
                result.Add(new BankAccount
                {
                    VendorId = VendorId.Hapoalim,
                    AccountId = accountPair.Key,
                    Label = accountPair.Value.ProductLabel,
                    UserId = _hapoalimApi.UserId,
                    BankName = BankName,
                    AccountNumber = accountPair.Value.AccountNumber,
                    BankNumber = accountPair.Value.BankNumber,
                    BranchNumber = accountPair.Value.BranchNumber,
                    Balance = GetBalance(accountPair.Key)
                });
            }

            return result;
        }

        public IEnumerable<ITransaction> GetTransactions(long accountId, DateTime startTime, DateTime endTime)
        {
            AccountResponse account;
            _accountsById.TryGetValue(accountId, out account);
            if (account == null)
            {
                throw new ArgumentOutOfRangeException(String.Format("There is no account with {0} account id", accountId));
            }

            var transactionsResult = _hapoalimApi.GetTransactions(account, startTime, endTime);
            if (!_transactions.ContainsKey(accountId))
            {
                _transactions.Add(accountId, new TransactionsResponse());
            }
            _transactions[accountId] = transactionsResult;

            var result = new List<ITransaction>();
            foreach (var transaction in transactionsResult.Transactions)
            {
                result.Add(new BankTransaction
                {
                    AccountId = accountId,
                    EventId = (long) (transaction.ReferenceNumber + Math.Round(transaction.EventAmount) + Math.Round(transaction.CurrentBalance)),
                    EventDate = new DateTime((int)transaction.EventDate / 10000, (int)transaction.EventDate / 100 % 100, (int)transaction.EventDate % 100).AddMinutes((int)transaction.ExpandedEventDate % 100),
                    EventDescription = transaction.ActivityDescription,
                    CurrentBalance = transaction.CurrentBalance,
                    EventAmount = transaction.EventAmount,
                    Type = transaction.EventActivityTypeCode == 1 ? TransactionType.Income : TransactionType.Expense,
                    SupplierId = transaction.ReferenceNumber.ToString(),
                });
            }

            return result;
        }

        public double GetBalance(long accountId)
        {
            AccountResponse account;
            _accountsById.TryGetValue(accountId, out account);
            if (account == null)
            {
                throw new ArgumentOutOfRangeException(String.Format("There is no account with {0} account id", accountId));
            }

            var balanceResult = _hapoalimApi.GetBalance(account);
            return balanceResult == null ? 0 : balanceResult.CurrentBalance;
        }

        public void RefreshData(long accountId, Func<bool> callback)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            _hapoalimApi.Dispose();
        }
    }
}
