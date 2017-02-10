using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using DataProvider.Interfaces;
using DataProvider.Model;
using HtmlAgilityPack;

namespace DataProvider.Cal
{
    public class CalApi : ICalApi
    {
        #region Constants
        private const string AspSessionId = "ASP.NET_SessionId";
        private const string AspAuth = ".ASPXAUTH";
        private const string ShivokInteger = "ShivokInteger";

        private const string ViewState = "__VIEWSTATE";
        private const string ViewStateGenerator = "__VIEWSTATEGENERATOR";
        private const string EventValidation = "__EVENTVALIDATION";
        private const string LoginDomain = "services.cal-online.co.il";
        #endregion

        private readonly string _username;
        private readonly string _password;
        private IList<string> _cards;
        private readonly CalSessionInfo _sessionInfo;
        public bool IsReady { get; }

        public CalApi(IProviderDescriptor accountDescriptor)
        {
            if (accountDescriptor == null)
            {
                throw new ArgumentNullException("credentials");
            }

            var crentialValues = accountDescriptor.Credentials.Values.ToArray();
            _username = crentialValues[0];
            _password = crentialValues[1];

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, policyErrors) => true;
            _sessionInfo = new CalSessionInfo();
            
            IsReady = true;
        }

        private void Init()
        {
            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/card-holders/Screens/AccountManagement/login.aspx");
            request.Method = "GET";
            string requestResult = SendRequest(request, null);
        }

        private void LogIn()
        {
            var body = string.Format(
                        "__EVENTTARGET=&" +
                        "__EVENTARGUMENT=&" +
                        "__VIEWSTATE={0}&" +
                        "__VIEWSTATEGENERATOR={1}&" +
                        "__EVENTVALIDATION={2}&" +
                        "GUID=&" +
                        "ReturnToUrl=&" +
                        "From=&ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24UserName={3}&" +
                        "ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24Password={4}&" +
                        "ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24LoginImageButton.x=63&ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24LoginImageButton.y=17&ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24txtUserID=&" +
                        "txtUserID_mandatoryHidden=%D7%99%D7%A9+%D7%9C%D7%9E%D7%9C%D7%90+%D7%AA.%D7%96.&" +
                        "txtUserID_customVldHidden=validateCustomID&" +
                        "txtUserID_regMsgHidden=%D7%9E%D7%A1%D7%A4%D7%A8+%D7%AA.%D7%96.+%D7%A9%D7%94%D7%95%D7%96%D7%9F+%D7%90%D7%99%D7%A0%D7%95+%D7%AA%D7%A7%D7%99%D7%9F.&" +
                        "ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24txtLast4Digits=&" +
                        "txtLast4Digits_mandatoryHidden=%D7%99%D7%A9+%D7%9C%D7%9E%D7%9C%D7%90+4+%D7%A1%D7%A4%D7%A8%D7%95%D7%AA+%D7%90%D7%97%D7%A8%D7%95%D7%A0%D7%95%D7%AA+%D7%A9%D7%9C+%D7%94%D7%9B%D7%A8%D7%98%D7%99%D7%A1.&" +
                        "txtLast4Digits_regExpHidden=%5E%5B0-9%5D%7B4%2C4%7D%24&" +
                        "txtLast4Digits_regMsgHidden=4+%D7%A1%D7%A4%D7%A8%D7%95%D7%AA+%D7%90%D7%97%D7%A8%D7%95%D7%A0%D7%95%D7%AA+%D7%A9%D7%9C+%D7%9B%D7%A8%D7%98%D7%99%D7%A1+%D7%90%D7%A9%D7%A8%D7%90%D7%99+%D7%90%D7%99%D7%A0%D7%9F+%D7%AA%D7%A7%D7%99%D7%A0%D7%95%D7%AA.&" +
                        "ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24txtChekingSigns=&" +
                        "txtChekingSigns_regExpHidden=%5E%5BA-Za-z%5D%7B6%7D%24&" +
                        "txtChekingSigns_regMsgHidden=%D7%94%D7%AA%D7%95%D7%95%D7%99%D7%9D+%D7%A9%D7%94%D7%95%D7%96%D7%A0%D7%95+%D7%90%D7%99%D7%A0%D7%9D+%D7%AA%D7%A7%D7%99%D7%A0%D7%99%D7%9D.+%D7%A0%D7%99%D7%AA%D7%9F+%D7%9C%D7%94%D7%99%D7%A2%D7%96%D7%A8+%D7%91%D7%94%D7%A1%D7%91%D7%A8+%D7%94%D7%9E%D7%95%D7%A6%D7%92+%D7%9C%D7%99%D7%93+%D7%94%D7%A9%D7%93%D7%94&" +
                        "txtChekingSigns_mandatoryHidden=%D7%94%D7%A7%D7%9C%D7%99%D7%93%D7%95+%D7%90%D7%AA+%D7%94%D7%AA%D7%95%D7%95%D7%99%D7%9D+%D7%94%D7%9E%D7%95%D7%A4%D7%99%D7%A2%D7%99%D7%9D+%D7%91%D7%AA%D7%9E%D7%95%D7%A0%D7%94&" +
                        "ctl00%24FormAreaNoBorder%24FormArea%24lgnLogin%24txtPassword=&" +
                        "txtPassword_mandatoryHidden=%D7%99%D7%A9+%D7%9C%D7%9E%D7%9C%D7%90+%D7%A1%D7%99%D7%A1%D7%9E%D7%90.&" +
                        "txtPassword_regExpHidden=%5E%5B0-9%5D%7B6%2C6%7D%24&" +
                        "txtPassword_regMsgHidden=%D7%94%D7%A1%D7%99%D7%A1%D7%9E%D7%90+%D7%A9%D7%94%D7%95%D7%96%D7%A0%D7%94+%D7%90%D7%99%D7%A0%D7%94+%D7%AA%D7%A7%D7%99%D7%A0%D7%94.&" +
                        "ctl00%24__MATRIX_VIEWSTATE=",
                        _sessionInfo.ViewState,
                        _sessionInfo.ViewStateGenerator,
                        _sessionInfo.EventValidation,
                        _username, _password);

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/card-holders/Screens/AccountManagement/login.aspx");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            
            string requestResult = SendRequest(request, bytes);
        }

        public IList<ICreditCardAccount> GetCards()
        {
            Init();
            LogIn();
            var requestResult = RetriveCards();
            Close();

            var result = new List<ICreditCardAccount>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(requestResult);

            var cards = doc.DocumentNode.Descendants("a").
                Where(a => a.Attributes.Contains("href") && a.Attributes["href"].Value.StartsWith("CardDetails.aspx?cardUniqueID=")).ToArray();

            _cards = new List<string>();
            foreach (var card in cards)
            {
                var index = card.Attributes["href"].Value.Split('=').LastOrDefault();
                var name = card.ChildNodes.Descendants("div").FirstOrDefault(a => a.Attributes.Contains("class") && a.Attributes["class"].Value.StartsWith("cardBrief1")).InnerText;
                var type = card.ChildNodes.Descendants("div").FirstOrDefault(a => a.Attributes.Contains("class") && a.Attributes["class"].Value.StartsWith("cardBrief2")).InnerText;
                var userName = name.Split(' ')[0];
                var number = name.Split(' ')[1];

                result.Add(new CreditCardAccount
                {
                    VendorId = VendorId.Cal,
                    AccountId = Convert.ToInt64(index),
                    CompanyName = "Visa-Cal",
                    UserId = _username,
                    Label = "Visa-Cal",
                    BankAccountId = 0,
                    BankBranchId = 0,
                    BankId = 0,
                    CardName = type,
                    CardNumber = Convert.ToInt64(number),
                    PartnerName = userName,
                    Balance = Double.NaN
                });

                _cards.Add(index);
            }
            
            return result;
        }
        
        private void RetriveDetails(string cardIndex)
        {
            WebRequest request = WebRequest.Create("https://services.cal-online.co.il/card-holders/Screens/AccountManagement/CardDetails.aspx?cardUniqueID=" + cardIndex);
            request.Method = "GET";
            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) { Domain = LoginDomain });
            var requestResult = SendRequest(request, null);
        }

        private void HomePage()
        {
            var body = string.Format(
               "__EVENTTARGET=ctl00%24AppTopMenu%24rptTopMenu%24ctl01%24HyperLink1&" +
               "__EVENTARGUMENT=&" +
               "__VIEWSTATE=&" +
               "GUID={0}&" +
               "ReturnToUrl=http%3A%2F%2Fservices.cal-online.co.il%2Fcard-holders%2FScreens%2FAccountManagement%2FHomePage.aspx&" +
               "From=CardHolders&" +
               "hiddenCardsImages=%5B%5D&" +
               "ctl00%24__MATRIX_VIEWSTATE=1",
               _sessionInfo.ShivokInteger);

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/card-holders/Screens/AccountManagement/HomePage.aspx");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) { Domain = LoginDomain });

            string requestResult = SendRequest(request, bytes);
        }

        public IList<ITransaction> GetTransactions(long cardIndex, int month, int year)
        {
            var index = $"0{cardIndex}";

            Init();
            LogIn();
            RetriveDetails(index);
            HomePage();
            var requestResult = RetriveTransactions(index, month, year);
            Close();

            var result = new List<ITransaction>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(requestResult);

            var tables = doc.DocumentNode.Descendants("tbody").ToArray();

            foreach (var table in tables)
            {
                if (!table.ChildNodes.Any(a => a.Attributes.Contains("onclick")))
                {
                    continue;
                }

                foreach (var transaction in table.ChildNodes)
                {
                    try
                    {
                        var properties = transaction.ChildNodes.Descendants("div").ToArray();
                        CreditTransaction newTransaction;
                        var id = transaction.Attributes["onclick"].Value.Split('|').LastOrDefault().Split('&').FirstOrDefault();
                        
                        if (transaction.ChildNodes.Count(a => a.Name.Equals("td")) == 5)
                        {
                            newTransaction = RetriveIsraelTransaction(cardIndex, id, properties);
                        }
                        else
                        {
                            newTransaction = RetriveForignTransaction(cardIndex, id, properties);
                        }

                        result.Add(newTransaction);
                    }
                    catch (Exception exp)
                    {
                    }
                }
            }

            return result;
        }

        private CreditTransaction RetriveIsraelTransaction(long cardIndex, string id, HtmlNode[] properties)
        {
            var date = properties[0].InnerText.Split('/');
            var description = properties[1].InnerText;
            var totalAmount = properties[2].InnerText.Split(' ').LastOrDefault();
            var amount = properties[3].InnerText.Split(' ').LastOrDefault();
            var plan = properties[4].InnerText;

            return new CreditTransaction
            {
                AccountId = cardIndex,
                CurrentBalance = Double.NaN,
                EventAmount = Math.Abs(Convert.ToDouble(amount)),
                EventDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0])),
                EventDescription = $"{description} {(string.IsNullOrEmpty(plan) ? "" : $"-{plan}")}",
                EventId = Convert.ToInt64(id),
                SupplierId = Math.Abs(description.GetHashCode()).ToString(),
                Type = Convert.ToDouble(amount) > 0 ? TransactionType.Expense : TransactionType.Income
            };
        }

        private CreditTransaction RetriveForignTransaction(long cardIndex, string id, HtmlNode[] properties)
        {
            var date = properties[1].InnerText.Split('/');
            var description = properties[2].InnerText;
            var totalAmount = properties[3].InnerText.Split(' ').LastOrDefault();
            var amount = properties[4].InnerText.Split(' ').LastOrDefault();
            var plan = properties[5].InnerText;
            
            return new CreditTransaction
            {
                AccountId = cardIndex,
                CurrentBalance = Double.NaN,
                EventAmount = Math.Abs(Convert.ToDouble(amount)),
                EventDate = new DateTime(Convert.ToInt32(date[2]), Convert.ToInt32(date[1]), Convert.ToInt32(date[0])),
                EventDescription = $"{description} {(string.IsNullOrEmpty(plan) ? "" : $"-{plan}")}",
                EventId = Convert.ToInt64(id),
                SupplierId = Math.Abs(description.GetHashCode()).ToString(),
                Type = Convert.ToDouble(amount) > 0 ? TransactionType.Expense : TransactionType.Income
            };
        }

        private string RetriveCards()
        {
            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/card-holders/Screens/AccountManagement/HomePage.aspx");
            request.Method = "GET";
            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) { Domain = LoginDomain });

            string requestResult = SendRequest(request, null);
            return requestResult;
        }

        private string RetriveTransactions(string cardIndex, int month, int year)
        {
            var lastMonthId = 19;
            var now = DateTime.Now;
            var currMonth = GetEffectiveMonth(now.Month);
            var currYear = GetEffectiveYear(now.Month, now.Year);
            var effMonth = GetEffectiveMonth(month);
            var effYear = GetEffectiveYear(month, year);
            var hiddenMonthInd = lastMonthId - (12 * (currYear - effYear) + (currMonth - effMonth));
            var cardHiddenInd = GetCardHiddenInd(cardIndex);
            
            var body = string.Format(
                "PrintTransDetailsRequest=&" +
                "__TRANS_DETAILS_PAGE_NAME=TransDetails.aspx&" +
                "hidToggleFormLink=AdvancedSearchClose&" +
                "__EVENTTARGET=SubmitRequest&" +
                "__EVENTARGUMENT=NewRequest&" +
                "__TRANS_SOURCE_NAME=Transactions&" +
                "__VIEWSTATE=&" +
                "cmbTransType_HiddenField=0&" +
                "__EVENTVALIDATION={0}&" +
                "GUID={1}&" +
                "ReturnToUrl=http%3A%2F%2Fservices.cal-online.co.il%2FCard-Holders%2FScreens%2FTransactions%2FTransactions.aspx&" +
                "From=CardHolders&" +
                "ctl00%24ContentTop%24cboCardList%24categoryList%24__categoryListSelectedValue=%7B%22group%22%3A%221%22%2C%22value%22%3A%22{2}%22%7D&" +
                "ctl00%24ContentTop%24cboCardList%24categoryList%24lblCollapse=%D7%90%D7%A0%D7%A1%D7%98%D7%A1%D7%99%D7%94+%28%D7%95%D7%99%D7%96%D7%94+%D7%96%D7%94%D7%91+%D7%A2%D7%A1%D7%A7%D7%99%29+-+4108&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24rdogrpTransactionType=rdoDebitDate&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24clndrDebitDateScope%24TextBox={3}%2F{4}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24clndrDebitDateScope%24HiddenField={6}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeStart%24ctlMonthYearList%24TextBox={3}%2F{4}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeStart%24ctlMonthYearList%24HiddenField={6}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeStart%24ctlDaysList%24TextBox=2&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeStart%24ctlDaysList%24HiddenField=1&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeEnd%24ctlMonthYearList%24TextBox={3}%2F{4}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeEnd%24ctlMonthYearList%24HiddenField={5}&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeEnd%24ctlDaysList%24TextBox=2&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24ctlDateScopeEnd%24ctlDaysList%24HiddenField=1&" +
                "cmbTransType=%D7%94%D7%9B%D7%9C&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24chkLocalOrigin=on&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24chkAbroadOrigin=on&" +
                "ctl00%24FormAreaNoBorder%24FormArea%24rdogrpSummaryReport=rdoAggregationNone&" +
                "ctl00%24__MATRIX_VIEWSTATE=3",
                _sessionInfo.EventValidation,
                _sessionInfo.ShivokInteger,
                cardIndex, effMonth, effYear, hiddenMonthInd, hiddenMonthInd-1);

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/Card-Holders/Screens/Transactions/Transactions.aspx");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            
            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) { Domain = LoginDomain });
            
            string requestResult = SendRequest(request, bytes);
            return requestResult;
        }

        private int GetCardHiddenInd(string cardIndex)
        {
            int ind = 1;
            foreach (var card in _cards)
            {
                if (card.Equals(cardIndex))
                {
                    return ind;
                }
                ind++;
            }

            return 0;
        }

        private void Close()
        {
            var body = string.Format(
                  "___EVENTTARGET=ctl00%24TopBox1%24lnkLogout&" +
                  "__EVENTARGUMENT=&" +
                  "__VIEWSTATE=&" +
                  "GUID={0}&" +
                  "ReturnToUrl=http%3A%2F%2Fservices.cal-online.co.il%2Fcard-holders%2FScreens%2FAccountManagement%2FHomePage.aspx&" +
                  "From=CardHolders&" +
                  "hiddenCardsImages=%5B%5D&" +
                  "ctl00%24__MATRIX_VIEWSTATE=1",
                  _sessionInfo.ShivokInteger);

            var encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);

            WebRequest request = WebRequest.Create(@"https://services.cal-online.co.il/card-holders/Screens/AccountManagement/HomePage.aspx");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) { Domain = LoginDomain });
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) { Domain = LoginDomain });

            string requestResult = SendRequest(request, bytes);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private string SendRequest(WebRequest request, byte[] bytes)
        {
            if (bytes != null)
            {
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
            }

            string responseBody;

            request.TryAddCookie(new Cookie(AspSessionId, _sessionInfo.AspSessionId) {Domain = LoginDomain});
            request.TryAddCookie(new Cookie(AspAuth, _sessionInfo.AspAuth) {Domain = LoginDomain});
            ((HttpWebRequest) request).AllowAutoRedirect = false;

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            try
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), true);
                try
                {
                    responseBody = streamReader.ReadToEnd();
                }
                finally
                {
                    streamReader.Close();
                }
            }
            finally
            {
                response.Close();
            }

            ExtractCookies(response);
            var stateView = ExtractAspEntity(responseBody, ViewState);
            var viewStateGenerator = ExtractAspEntity(responseBody, ViewStateGenerator);
            var eventValidation = ExtractAspEntity(responseBody, EventValidation);

            if (!string.IsNullOrEmpty(stateView))
            {
                _sessionInfo.ViewState = stateView;
            }
            if (!string.IsNullOrEmpty(viewStateGenerator))
            {
                _sessionInfo.ViewStateGenerator = viewStateGenerator;
            }
            if (!string.IsNullOrEmpty(eventValidation))
            {
                _sessionInfo.EventValidation = eventValidation;
            }

            var location = response.GetResponseHeader("Location");
            if (!string.IsNullOrWhiteSpace(location))
            {
                WebRequest req = WebRequest.Create("https://services.cal-online.co.il" + location);
                req.Method = "GET";
                SendRequest(req, null);
            }

            return responseBody;
        }
        
        private void ExtractCookies(HttpWebResponse response)
        {
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

                    if (nameValue[0].Equals(AspSessionId))
                    {
                        _sessionInfo.AspSessionId = nameValue[1];
                    }
                    else if (nameValue[0].Equals(AspAuth))
                    {
                        _sessionInfo.AspAuth = nameValue[1];
                    }
                    else if (nameValue[0].Equals(ShivokInteger))
                    {
                        _sessionInfo.ShivokInteger = nameValue[1];
                    }
                }
            }
        }

        private static string ExtractAspEntity(string data, string entity)
        {
            const string valueDelimiter = "value=\"";
            try
            {
                int viewStateNamePosition = data.IndexOf(entity, StringComparison.Ordinal);
                if (viewStateNamePosition == -1)
                {
                    return string.Empty;
                }

                int viewStateValuePosition = data.IndexOf(valueDelimiter, viewStateNamePosition, StringComparison.Ordinal);
                int viewStateStartPosition = viewStateValuePosition + valueDelimiter.Length;
                int viewStateEndPosition = data.IndexOf("\"", viewStateStartPosition, StringComparison.Ordinal);
                return HttpUtility.UrlEncode(data.Substring(viewStateStartPosition, viewStateEndPosition - viewStateStartPosition));
            }
            catch (Exception exp)
            {
            }

            return string.Empty;
        }

        private int GetEffectiveMonth(int month)
        {
            return month == 12 ? 1 : month + 1;
        }

        private int GetEffectiveYear(int month, int year)
        {
            return month == 12 ? year + 1 : year;
        }
    }
}
