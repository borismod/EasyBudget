using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DataProvider.Hapoalim;
using DataProvider.Hapoalim.Responses;
using DataProvider.Interfaces;
using DataProvider.Model;
using Newtonsoft.Json;

namespace DataProvider.Umtb
{
    public class UmtbDataProvider : IDataProvider
    {
        private string _userId;
        private string _userCredentials;
        private UmtbSessionInfo _sessionInfo;
        private string BIGipServerlogin = "BIGipServerlogin-web-ca-Pool";
        private string TS01eef807 = "TS01eef807";
        private const string AspnetSessionId = "ASP.NET_SessionId";

        public UmtbDataProvider(IProviderDescriptor accountDescriptor)
        {
            if (accountDescriptor == null || accountDescriptor.Credentials.Count == 0)
            {
                throw new ArgumentNullException("accountDescriptor");
            }

            var crentialValues = accountDescriptor.Credentials.Values.ToArray();
            _userId = crentialValues[0];
            _userCredentials = crentialValues[1];

        }

        public void Dispose()
        {
        }

        public string Name => "Bank Mizrahi Tefahot";
        public bool IsReady => true;
        public IEnumerable<IAccount> GetAccounts()
        {
            return new[]
            {
                new BankAccount()
                {

                }
            };
        }

        public IEnumerable<ITransaction> GetTransactions(long accountId, DateTime startTime, DateTime endTime)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://www.mizrahi-tefahot.co.il/login/login2MTO.aspx");


            byte[] bodyBuffer = Encoding.UTF8.GetBytes($"Username={_userId}&Password={_userCredentials}&Lang=HE");

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent =
                @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.75 Safari/537.36";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.ContentLength = bodyBuffer.Length;
            request.GetRequestStream().Write(bodyBuffer, 0, bodyBuffer.Length);
            WebHeaderCollection headers = request.Headers;
            headers.Add("Accept-Encoding:gzip, deflate, br");
            headers.Add("Accept-Language:en-US,en;q=0.8,he;q=0.6,ru;q=0.4");

            //request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            //request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            //request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            //request.TryAddCookie(new Cookie(Token, _sessionInfo.Token) { Domain = LoginDomain });
            //request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
            var accountsResponse = JsonConvert.DeserializeObject<IList<AccountResponse>>(result);

            return null;
        }

        private string SendRequest(WebRequest request, byte[] bytes)
        {
            HttpWebResponse response = null;
            Stream dataStream = null;
            try
            {
                if (bytes != null)
                {
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(bytes, 0, bytes.Length);
                }

                response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.StatusDescription);
                dataStream = response.GetResponseStream();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            if (dataStream == null)
            {
                return string.Empty;
            }

            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            ExtractCookies(response);

            reader.Close();
            dataStream.Close();
            response.Close();

            Console.WriteLine(responseFromServer);
            return responseFromServer;
        }

        private void ExtractCookies(HttpWebResponse response)
        {
            if (_sessionInfo == null)
            {
                _sessionInfo = new UmtbSessionInfo();
            }

            var setValues = response.Headers.GetValues("set-cookie");
            if (setValues == null)
            {
                return;
            }

            foreach (var setValue in setValues)
            {
                var items = setValue.Split(';');
                foreach (var item in items)
                {
                    var nameValue = item.Split('=');

                    if (nameValue[0].Equals(AspnetSessionId))
                    {
                        _sessionInfo.AspnetSessionId  = nameValue[1];
                    }
                    else if (nameValue[0].Equals(BIGipServerlogin))
                    {
                        _sessionInfo.BIGipServerlogin = nameValue[1];
                    }
                    else if (nameValue[0].Equals(TS01eef807))
                    {
                        _sessionInfo.TS01eef807 = nameValue[1];
                    }
                }
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
    }

    public interface IUmtbApi
    {
    }
}