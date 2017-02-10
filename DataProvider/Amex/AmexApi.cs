using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataProvider.Amex.Requests;
using DataProvider.Amex.Responses;
using DataProvider.Interfaces;
using HtmlAgilityPack;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProvider.Amex
{
    public class AmexApi : IAmexApi
    {
        #region Constants
        private const string LoginDomain = ".americanexpress.co.il";
        private const string Jsessionid = "JSESSIONID";
        private const string Alt50_ZLinuxPrd = "Alt50_ZLinuxPrd";
        private const string ServiceP = "ServiceP";
        private const string RequestVerificationToken = "__RequestVerificationToken";
        #endregion

        private readonly string _idNumber;
        private readonly string _lastDigits;
        private readonly string _password;

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AmexSessionInfo _sessionInfo;
        public bool IsReady { get; }
        public string UserId {
            get { return _idNumber; } 
        }

        public AmexApi(IProviderDescriptor accountDescriptor)
        {
            if (accountDescriptor == null)
            {
                throw new ArgumentNullException("credentials");
            }

            var crentialValues = accountDescriptor.Credentials.Values.ToArray();
            _idNumber = crentialValues[0];
            _lastDigits = crentialValues[1];
            _password = crentialValues[2];

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, policyErrors) => true;

            IsReady = true;
            var loginResponse = Login();

            if (loginResponse.Status != 1)
            {
                IsReady = false;

                _log.ErrorFormat("Cannot create AmexApi - {0}", loginResponse.Message);
                throw new FieldAccessException(loginResponse.Message);
            }

            _log.Debug("AmexApi is created");
        }

        public CardListDeatils GetCards()
        {
            if (!IsReady)
            {
                return new CardListDeatils();
            }

            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/services/ProxyRequestHandler.ashx?reqName=CardsList_102Digital");
            request.Headers.Add(RequestVerificationToken, _sessionInfo.RequestVerificationTokenInput);
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(RequestVerificationToken, _sessionInfo.RequestVerificationToken) { Domain = LoginDomain });
            request.Method = "GET";

            string result = SendRequest(request, null);
            var data = JsonConvert.DeserializeObject<CardListResponse>(result);
            if (IsError(data.Header))
            {
                throw new Exception($"Cannot to get cards. Error: {GetError(data.Header)}");
            }
            return data.CardsList_102DigitalBean;
        }

        public IEnumerable<CardTransaction> GetTransactions(long cardIndex, int month, int year)
        {
            if (!IsReady)
            {
                return Enumerable.Empty<CardTransaction>();
            }

            var effMonth = GetEffectiveMonth(month);
            var effYear = GetEffectiveYear(month, year);
            var arguments = $"&month={effMonth.ToString("00")}&year={effYear}&cardIdx={cardIndex}";
            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/services/ProxyRequestHandler.ashx?reqName=CardsTransactionsList" + arguments);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(RequestVerificationToken, _sessionInfo.RequestVerificationTokenInput);
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(RequestVerificationToken, _sessionInfo.RequestVerificationToken) { Domain = LoginDomain });

            string response = SendRequest(request, null);
            return GetDealResponses(response);
        }

        private int GetEffectiveMonth(int month)
        {
            return month == 12 ? 1 : month + 1;
        }

        private int GetEffectiveYear(int month, int year)
        {
            return month == 12 ? year + 1 : year;
        }

        private static string FormatTransactionsList(JObject cardsTransactionsList, string json, string pattern, string newText)
        {
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            List<JProperty> keys = cardsTransactionsList.Properties().Where(p =>
            {
                Match match = regex.Match(p.Name);
                return match.Success;
            }).ToList();

            string result = string.Empty;
            foreach (var key in keys)
            {
                result = json.Replace(key.Name, newText);
            }

            return result;
        }

        internal static IEnumerable<CardTransaction> GetDealResponses(string response)
        {
            string result = response;
            JObject json = JObject.Parse(result);

            var cardsTransactionsListProperty = json.Property("CardsTransactionsListBean");
            if (cardsTransactionsListProperty == null)
            {
                throw new Exception($"Cannot to get transactions. Error: CardsTransactionsListBean is missing");
            }

            var cardsTransactionsList = cardsTransactionsListProperty.Value as JObject;

            result = FormatTransactionsList(cardsTransactionsList, result, @"^card[0-9]*$", "card");
            result = FormatTransactionsList(cardsTransactionsList, result, @"^index[0-9]*$", "index");
            result = FormatTransactionsList(cardsTransactionsList, result, @"^id[0-9]+$", "id");

            var data = JsonConvert.DeserializeObject<TransactionsListResponse>(result);
            if (IsError(data.Header))
            {
                throw new Exception($"Cannot to get transactions. Error: {GetError(data.Header)}");
            }
            var transactions = data.CardsTransactionsListBean.Index.CurrentCardTransactions.First().TxnIsrael;
            return transactions ?? new List<CardTransaction>();
        }
        
        private LoginResponse Login()
        {
            GetVerificationToken();

            var bodyObject = new LogonBody
            {
                KodMishtamesh = GetCodeMishtamesh(),
                MisparZihuy = _idNumber,
                Sisma = _password
            };

            var body = JsonConvert.SerializeObject(bodyObject);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/services/ProxyRequestHandler.ashx?reqName=performLogonA");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.Headers.Add(RequestVerificationToken, _sessionInfo.RequestVerificationTokenInput);
            request.TryAddCookie(new Cookie(RequestVerificationToken, _sessionInfo.RequestVerificationToken) { Domain = LoginDomain });

            string response = SendRequest(request, bytes);
            var result = JsonConvert.DeserializeObject<LoginResponse>(response);
            return result;
        }

        private void GetVerificationToken()
        {
            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/personalarea/login/");
            request.Method = "GET";
            string response = SendRequest(request, null);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            var t = doc.DocumentNode.Descendants("input").
                FirstOrDefault(a => a.Attributes.Contains("name") && a.Attributes["name"].Value.Equals(RequestVerificationToken));
            _sessionInfo.RequestVerificationTokenInput = t.Attributes["value"].Value;
        }

        private string GetCodeMishtamesh()
        {
            var bodyObject = new ValidateIdBody
            {
                CountryCode = "212",
                ApplicationSource = "0",
                CompanyCode = "77",
                IdType = "1",
                CheckLevel = "1",
                Id = _idNumber,
                CardSuffix = _lastDigits
            };

            var body = JsonConvert.SerializeObject(bodyObject);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/services/ProxyRequestHandler.ashx?reqName=ValidateIdData");
            request.Headers.Add(RequestVerificationToken, _sessionInfo.RequestVerificationTokenInput);
            request.TryAddCookie(new Cookie(RequestVerificationToken, _sessionInfo.RequestVerificationToken) { Domain = LoginDomain });
            request.Method = "POST";
            
            string response = SendRequest(request, bytes);
            var data = JsonConvert.DeserializeObject<ValidateIdResponse>(response);
            if (data == null || IsError(data.Header))
            {
                throw new Exception($"Cannot to code mishtamesh. Error: {GetError(data != null ? data.Header : new HeaderResponse())}");
            }

            return data.ValidateIdDataBean.UserName;
        }

        private void Exit()
        {
            WebRequest request = WebRequest.Create(@"https://he.americanexpress.co.il/services/ProxyRequestHandler.ashx?reqName=performExit");
            request.Headers.Add(RequestVerificationToken, _sessionInfo.RequestVerificationTokenInput);
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(RequestVerificationToken, _sessionInfo.RequestVerificationToken) { Domain = LoginDomain });

            request.Method = "POST";

            string result = SendRequest(request, null);
        }

        private string SendRequest(WebRequest request, byte[] bytes)
        {
            HttpWebResponse response = null;
          
            if (bytes != null)
            {
                try
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                    using (Stream newStream = request.GetRequestStream())
                    {
                        newStream.Write(bytes, 0, bytes.Length);
                        response = (HttpWebResponse) request.GetResponse();
                        newStream.Close();
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                }
            }
            else
            {
                response = (HttpWebResponse)request.GetResponse();
            }

            if (response == null)
            {
                return string.Empty;
            }

            Console.WriteLine(response.StatusDescription);
            Stream dataStream = response.GetResponseStream();
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

        private static bool IsError(HeaderResponse header)
        {
            return header == null || header.Status != 1;
        }

        private static string GetError(HeaderResponse header)
        {
            if (header.Status == 0)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(header?.Message))
            {
                return "Unknown error";
            }

            return header.Message;
        }

        private void ExtractCookies(HttpWebResponse response)
        {
            if (_sessionInfo == null)
            {
                _sessionInfo = new AmexSessionInfo();
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

                    if (nameValue[0].Equals(Jsessionid))
                    {
                        _sessionInfo.Jsessionid = nameValue[1];
                    }
                    else if (nameValue[0].Equals(Alt50_ZLinuxPrd))
                    {
                        _sessionInfo.Alt50_ZLinuxPrd = nameValue[1];
                    }
                    else if (nameValue[0].Equals(ServiceP))
                    {
                        _sessionInfo.ServiceP = nameValue[1];
                    }
                    else if (nameValue[0].Equals(RequestVerificationToken))
                    {
                        _sessionInfo.RequestVerificationToken = nameValue[1];
                    }

                    
                }
            }
        }

        public void Dispose()
        {
            Exit();
        }
    }
}
