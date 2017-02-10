using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Schema;
using DataProvider.Amex.Responses;
using DataProvider.Amex;
using DataProvider.Interfaces;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProvider.Amex
{
    internal class MockAmexApi : IAmexApi
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _isValid;
        private static readonly BitArray _validity = new BitArray(4);

        public bool IsReady => _isValid;

        public string UserId
        {
            get
            {
                return "id";
            }
        }

        public MockAmexApi(IProviderDescriptor accountDescriptor)
        {
            Thread.Sleep(1500);
            if (accountDescriptor.Credentials.ContainsKey("Id") && 
                (accountDescriptor.Credentials["Id"].Equals("323764423") || accountDescriptor.Credentials["Id"].Equals("311913289")))
            {
                _isValid = true;
            }
            else
            {
                _isValid = false;
            }

            _log.Debug("MockAmexApi is created");
        }

        public CardListDeatils GetCards()
        {
            if (!_isValid)
            {
                return new CardListDeatils();
            }

            Thread.Sleep(100);
            return JsonConvert.DeserializeObject<CardListDeatils>(MockData.AmexCards);
        }

        public IEnumerable<CardTransaction> GetTransactions(long cardIndex, int month, int year)
        {
            Thread.Sleep(250);
            if (!_isValid)
            {
                return new List<CardTransaction>();
            }

            switch (cardIndex)
            {
                case 0:
                    if (_validity.Get(0))
                    {
                        return new List<CardTransaction>();
                    }
                    _validity.Set(0, true);
                    return AmexApi.GetDealResponses(MockData.CardTransactions1);
                case 3:
                    if (_validity.Get(3))
                    {
                        return new List<CardTransaction>();
                    }
                    _validity.Set(3, true);
                    return AmexApi.GetDealResponses(MockData.CardTransactions2);
            }

            return new List<CardTransaction>();
        }

        public void Dispose()
        {
        }
    }
}
