using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DataProvider.Hapoalim.Responses;
using DataProvider.Interfaces;
using log4net;
using log4net.Config;
using Newtonsoft.Json;

namespace DataProvider.Hapoalim
{
    public class HapoalimApi : IHapoalimApi
    {
        private readonly string _userId;
        private readonly string _userCredentials;

        #region Constants
        private const string DefaultOrganisation = "106402333";
        private const string LoginDomain = "login.bankhapoalim.co.il";
        private const string ActiveUser = "activeUser";
        private const string Smsession = "SMSESSION";
        private const string Jsessionid = "JSESSIONID";
        private const string Token = "token";
        private const string Lbinfologin = "lbinfologin";
        #endregion

        #region Fields
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private InitLoginResponse _initInfo;
        private SessionInfo _sessionInfo;
        #endregion

        public bool IsReady { get; }
        public string UserId {
            get { return _userId; }
        }

        public HapoalimApi(IProviderDescriptor accountDescriptor)
        {
            if (accountDescriptor == null || accountDescriptor.Credentials.Count == 0)
            {
                throw new ArgumentNullException("accountDescriptor");
            }

            var crentialValues = accountDescriptor.Credentials.Values.ToArray();
            _userId = crentialValues[0];
            _userCredentials = crentialValues[1];

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, policyErrors) => true;

            // Acquire token
            IsReady = true;
            _initInfo = Init();
            var result = Verify();
            if (result == null || result.Error != null)
            {
                IsReady = false;
            }

            GetLandpage();

            _log.Debug("HapoalimApi is created");
        }

        public IEnumerable<AccountResponse> GetAccountsData()
        {
            if (!IsReady)
            {
                return Enumerable.Empty<AccountResponse>();
            }

            WebRequest request = WebRequest.Create(@"https://login.bankhapoalim.co.il/ServerServices/general/accounts");

            request.Method = "GET";
            request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Token, _sessionInfo.Token) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
            var accountsResponse = JsonConvert.DeserializeObject<IList<AccountResponse>>(result);
            return accountsResponse;
        }

        public TransactionsResponse GetTransactions(AccountResponse account, DateTime startTime, DateTime endTime)
        {
            if (!IsReady)
            {
                return new TransactionsResponse();
            }

            var date1 = string.Format("{0}{1}{2}", startTime.Year, startTime.Month.ToString("00"), startTime.Day.ToString("00"));
            var date2 = string.Format("{0}{1}{2}", endTime.Year, endTime.Month.ToString("00"), endTime.Day.ToString("00"));
            string url = string.Format(@"https://login.bankhapoalim.co.il/ServerServices/current-account/transactions?accountId={0}-{1}-{2}&retrievalEndDate={3}&retrievalStartDate={4}",
              account.BankNumber, account.BranchNumber, account.AccountNumber, date2, date1);

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Token, _sessionInfo.Token) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
            return string.IsNullOrEmpty(result) ? new TransactionsResponse() : JsonConvert.DeserializeObject<TransactionsResponse>(result);
        }

        public BalanceResponse GetBalance(AccountResponse account)
        {
            if (!IsReady)
            {
                return new BalanceResponse();
            }

            string url = string.Format(@"https://login.bankhapoalim.co.il/ServerServices/current-account/composite/balanceAndCreditLimit?accountId={0}-{1}-{2}",
              account.BankNumber, account.BranchNumber, account.AccountNumber);

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Token, _sessionInfo.Token) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
            return JsonConvert.DeserializeObject<BalanceResponse>(result);
        }

        #region Private Methods
        private InitLoginResponse Init()
        {
            var body = string.Format("identifier={0}&organization={1}", _userId, DefaultOrganisation);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://login.bankhapoalim.co.il/authenticate/init");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = bytes.Length;

            string result = SendRequest(request, bytes);
            return JsonConvert.DeserializeObject<InitLoginResponse>(result);
        }

        private VerifyResponse Verify()
        {
            var body = string.Format("identifier={0}&Language=&bsd=&userID={1}&instituteCode={2}&credentials={3}&organization={4}&G3CmE=&authType=VERSAFE&flow=&state=",
                                     _userId, _userId, DefaultOrganisation, _userCredentials, DefaultOrganisation);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://login.bankhapoalim.co.il/authenticate/verify");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = bytes.Length;

            string result = SendRequest(request, bytes);
            return JsonConvert.DeserializeObject<VerifyResponse>(result);
        }

        private void GetLandpage()
        {
            if (!IsReady)
            {
                return;
            }

            WebRequest request = WebRequest.Create(@"https://login.bankhapoalim.co.il/AUTHENTICATE/LANDPAGE?flow=AUTHENTICATE&state=LANDPAGE&reqName=MainFrameSet");

            request.Method = "GET";
            request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
        }

        private void Exit()
        {
            if (!IsReady)
            {
                return;
            }

            WebRequest request = WebRequest.Create(@"https://login.bankhapoalim.co.il/portalserver/logoff?portalName=retailbanking");

            request.Method = "GET";
            request.TryAddCookie(new Cookie(ActiveUser, _sessionInfo.ActiveUser) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Smsession, _sessionInfo.Smsession) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Token, _sessionInfo.Token) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Lbinfologin, _sessionInfo.Lbinfologin) { Domain = LoginDomain });

            string result = SendRequest(request, null);
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
                _sessionInfo = new SessionInfo();
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

                    if (nameValue[0].Equals(Smsession))
                    {
                        _sessionInfo.Smsession = nameValue[1];
                    }
                    else if (nameValue[0].Equals(Lbinfologin))
                    {
                        _sessionInfo.Lbinfologin = nameValue[1];
                    }
                    else if (nameValue[0].Equals(ActiveUser))
                    {
                        _sessionInfo.ActiveUser = nameValue[1];
                    }
                    else if (nameValue[0].Equals(Jsessionid))
                    {
                        _sessionInfo.Jsessionid = nameValue[1];
                    }
                    else if (nameValue[0].Equals(Token))
                    {
                        _sessionInfo.Token = nameValue[1];
                    }
                }
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            Exit();
        }
        #endregion
    }
}
