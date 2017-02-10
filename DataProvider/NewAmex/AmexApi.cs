using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataProvider.Amex.Responses;
using DataProvider.Interfaces;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataProvider.Amex
{
    public class AmexApi : IAmexApi
    {
        #region Constants
        private readonly string _misparZihuy;
        private readonly string _kodMishtamesh;
        private readonly string _sisma;

        private const string ClientDevice = "Other";
        private const string LoginDomain = "service.isracard.co.il";
        private const string Jsessionid = "JSESSIONID";
        private const string Alt50_ZLinuxPrd = "Alt50_ZLinuxPrd";
        private const string ServiceP = "ServiceP";
        #endregion

        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AmexSessionInfo _sessionInfo;
        public bool IsReady { get; }

        public AmexApi(IProviderDescriptor accountDescriptor)
        {
            if (accountDescriptor == null)
            {
                throw new ArgumentNullException("credentials");
            }

            var crentialValues = accountDescriptor.Credentials.Values.ToArray();
            _misparZihuy = crentialValues[0];
            _kodMishtamesh = crentialValues[1];
            _sisma = crentialValues[2];

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, policyErrors) => true;

            IsReady = true;
            var initResult = Init();
            JObject json = JObject.Parse(initResult);
            if (IsError(json))
            {
                IsReady = false;
                var error = GetError(json);

                _log.ErrorFormat("Cannot create AmexApi - {0}", error);
                throw new FieldAccessException(error);
            }

            _log.Debug("AmexApi is created");
        }

        public CardListResponse GetCards()
        {
            if (!IsReady)
            {
                return new CardListResponse();
            }

            WebRequest request = WebRequest.Create(@"https://service.isracard.co.il/ajaxServlet/amex?reqName=CardsList_102");
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });

            request.Method = "POST";

            string result = SendRequest(request, null);
            JObject json = JObject.Parse(result);

            var headerJson = json.GetValue("CardsList_102Bean");
            return JsonConvert.DeserializeObject<CardListResponse>(headerJson.ToString());
        }

        public IEnumerable<DealResponse> GetTransactions(long cardIndex)
        {
            if (!IsReady)
            {
                return Enumerable.Empty<DealResponse>();
            }

            var body = string.Format("isFuturesDebits={0}&CardIndex={1}&selectedDate={2}", "true", cardIndex, 23);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://service.isracard.co.il/ajaxServlet/amex?reqName=DealsListForDate_203");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });

            string requestResult = SendRequest(request, bytes);
            JObject json = JObject.Parse(requestResult);
            if (IsError(json))
            {
                throw new Exception("Cannot to get cards");
            }

            return GetDealResponses(json);
        }

        public DealDetailsResponse GetTransactionDetails(long cardIndex, string date, string dealId)
        {
            if (!IsReady)
            {
                return new DealDetailsResponse();
            }

            var body = string.Format("CardIndex={0}&inState={1}&moedChiuv={2}&shovarRatz={3}", cardIndex, "yes", date, dealId);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://service.isracard.co.il/ajaxServlet/amex?reqName=PirteyIska_204");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });

            string requestResult = SendRequest(request, bytes);
            JObject json = JObject.Parse(requestResult);
            if (IsError(json))
            {
                throw new Exception("Cannot to get cards");
            }

            return GetDealDetailsResponses(json);
        }

        internal static IEnumerable<DealResponse> GetDealResponses(JObject json)
        {
            JToken dealsListJson;
            json.TryGetValue("DealsListForDate_203Bean", StringComparison.InvariantCultureIgnoreCase, out dealsListJson);
            if (dealsListJson == null)
            {
                return new List<DealResponse>();
            }

            var dealsList = dealsListJson as JObject;
            var tables = dealsList.Children().Where(c =>
            {
                var propertyName = c as JProperty;
                if (propertyName == null)
                {
                    return false;
                }
                return propertyName.Name.StartsWith("Table");
            });

            var result = new List<DealResponse>();
            foreach (var table in tables)
            {
                var deals = JsonConvert.DeserializeObject<IList<DealResponse>>(((JProperty)table).Value.ToString());
                foreach (var deal in deals)
                {
                    result.Add(deal);
                }
            }

            return result;
        }

        internal static DealDetailsResponse GetDealDetailsResponses(JObject json)
        {
            JToken dealsListJson;
            json.TryGetValue("PirteyIska_204Bean", StringComparison.InvariantCultureIgnoreCase, out dealsListJson);
            if (dealsListJson == null)
            {
                return null;
            }

            var details = dealsListJson as JObject;
            var result = JsonConvert.DeserializeObject<DealDetailsResponse>(details.ToString());
            return result;
        }

        private string Init()
        {
            var body = string.Format("MisparZihuy={0}&KodMishtamesh={1}&Sisma={2}&clientDevice={3}", _misparZihuy, _kodMishtamesh, _sisma, ClientDevice);
            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://service.isracard.co.il/ajaxServlet/amex?reqName=performLogonA");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            string result = SendRequest(request, bytes);
            return result;
        }

        private void Exit()
        {
            WebRequest request = WebRequest.Create(@"https://service.isracard.co.il/ajaxServlet/amex?reqName=performExitA");
            request.TryAddCookie(new Cookie(Jsessionid, _sessionInfo.Jsessionid) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(Alt50_ZLinuxPrd, _sessionInfo.Alt50_ZLinuxPrd) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(ServiceP, _sessionInfo.ServiceP) { Domain = LoginDomain });

            request.Method = "POST";

            string result = SendRequest(request, null);
        }

        private string SendRequest(WebRequest request, byte[] bytes)
        {
            if (bytes != null)
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

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

        private bool IsError(JObject result)
        {
            var headerJson = result.GetValue("Header");
            var header = JsonConvert.DeserializeObject<HeaderResponse>(headerJson.ToString());
            return header == null || header.Status != 1;
        }

        private string GetError(JObject result)
        {
            var headerJson = result.GetValue("Header");
            var header = JsonConvert.DeserializeObject<HeaderResponse>(headerJson.ToString());
            if (header == null)
            {
                return "Unknown error";
            }

            return header.Status != 1 ? (string)header.Message : string.Empty;
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
                }
            }
        }

        public void Dispose()
        {
            Exit();
        }
    }
}
