using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataProvider.Interfaces;
using log4net;

namespace DataProvider.Cal
{
    public class CalDataProvider : IDataProvider
    {
        private const string CompanyName = "Cal";

        private readonly ICalApi _calApi;

        public bool IsReady => _calApi.IsReady;
        public string Name => CompanyName;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CalDataProvider(ICalApi calApi)
        {
            _calApi = calApi;
            _log.Debug("CalDataProvider is created");
        }

        public IEnumerable<IAccount> GetAccounts()
        {
            var cards = _calApi.GetCards();
            return cards;
        }

        public IEnumerable<ITransaction> GetTransactions(long accountId, DateTime startTime, DateTime endTime)
        {
            lock (CompanyName)
            {
                var transactions = _calApi.GetTransactions(accountId, startTime.Month, startTime.Year);
                return transactions;
            }
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
            throw new NotImplementedException();
        }
    }
}
