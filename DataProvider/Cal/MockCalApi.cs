using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataProvider.Interfaces;
using DataProvider.Model;
using log4net;

namespace DataProvider.Cal
{
    internal class MockCalApi : ICalApi
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsReady { get; }

        public MockCalApi()
        {
            _log.Debug("MockCalApi is created");
            IsReady = true;
        }

        public IList<ICreditCardAccount> GetCards()
        {
            Thread.Sleep(200);

            return new List<ICreditCardAccount>
            {
                new CreditCardAccount
                {
                    VendorId = VendorId.Cal,
                    AccountId = 01874108,
                    CompanyName = "Visa-Cal",
                    UserId = "User",
                    Label = "Visa-Cal",
                    BankAccountId = 0,
                    BankBranchId = 0,
                    BankId = 0,
                    CardName = "Visa-Cal Card",
                    CardNumber = 5644456,
                    PartnerName = "Patner",
                    Balance = Double.NaN
                },
                new CreditCardAccount
                {
                    VendorId = VendorId.Cal,
                    AccountId = 01571802,
                    CompanyName = "Visa-Cal",
                    UserId = "User",
                    Label = "Visa-Cal",
                    BankAccountId = 0,
                    BankBranchId = 0,
                    BankId = 0,
                    CardName = "Visa-Cal Card",
                    CardNumber = 1231,
                    PartnerName = "Patner",
                    Balance = Double.NaN
                }
            };
        }

        public IList<ITransaction> GetTransactions(long cardIndex, int month, int year)
        {
            Thread.Sleep(300);

            return new List<ITransaction>();
        }

        public void Dispose()
        {
            
        }
    }
}
