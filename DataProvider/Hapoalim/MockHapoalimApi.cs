using System;
using System.Collections.Generic;
using System.Threading;
using DataProvider.Hapoalim.Responses;
using DataProvider.Interfaces;
using log4net;
using Newtonsoft.Json;

namespace DataProvider.Hapoalim
{
    internal class MockHapoalimApi : IHapoalimApi
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly bool _isValid;
        public bool IsReady => _isValid;
        public string UserId {
            get { return "id"; }
        }

        public MockHapoalimApi(IProviderDescriptor accountDescriptor)
        {
            Thread.Sleep(300);
            if (accountDescriptor.Credentials.ContainsKey("Password") &&
                accountDescriptor.Credentials["Password"].Equals("12345678"))
            {
                _isValid = true;
            }
            else
            {
                _isValid = false;
            }

            _log.Debug("MockHapoalimApi is created");
        }

        public IEnumerable<AccountResponse> GetAccountsData()
        {
            if (!_isValid)
            {
                return new List<AccountResponse>();
            }

            Thread.Sleep(200);
            string json = MockData.HapoalimAccounts;
            var accountsResponse = JsonConvert.DeserializeObject<IList<AccountResponse>>(json);
            return accountsResponse;
        }

        public TransactionsResponse GetTransactions(AccountResponse account, DateTime startTime, DateTime endTime)
        {
            Thread.Sleep(300);
            if (!_isValid)
            {
                return new TransactionsResponse();
            }

            string json = String.Empty;
            switch (account.BranchNumber)
            {
                case 135:
                    json = MockData.HapoalimTransactions1;
                    break;
                case 345:
                    json = MockData.HapoalimTransactions2;
                    break;
                case 545:
                    json = MockData.HapoalimTransactions3;
                    break;
            }

            var transactionResponce = JsonConvert.DeserializeObject<TransactionsResponse>(json);
            //for (int i = 0; i < transactionResponce.Transactions.Count; i++)
            //{
            //    if (transactionResponce.Transactions[i].EventDate < (startTime.Year * 100 + startTime.Month) * 100 + startTime.Day ||
            //        transactionResponce.Transactions[i].EventDate > (endTime.Year * 100 + endTime.Month) * 100 + endTime.Day)
            //    {
            //        transactionResponce.Transactions.RemoveAt(i);
            //        i--;
            //    }
            //}
            
            return transactionResponce;
        }

        public BalanceResponse GetBalance(AccountResponse account)
        {
            if (!_isValid)
            {
                return new BalanceResponse();
            }

            string json = String.Empty;
            switch (account.BranchNumber)
            {
                case 135:
                    json = MockData.HapoalimBalance1;
                    break;
                case 345:
                    json = MockData.HapoalimBalance2;
                    break;
                case 545:
                    json = MockData.HapoalimBalance3;
                    break;
            }

            return JsonConvert.DeserializeObject<BalanceResponse>(json);
        }
        public void Dispose()
        {
        }
    }
}
