using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataProvider.Amex.Responses
{
    public class ValidateIdResponse
    {
        public HeaderResponse Header { get; set; }

        public ValidateDataBeanResponse ValidateIdDataBean { get; set; }
    }

    public class ValidateDataBeanResponse
    {
        [JsonProperty(PropertyName = "authorizationsStatusDescription")]
        public object AuthorizationsStatusDescription { get; set; }

        [JsonProperty(PropertyName = "authorizationStatus")]
        public int AuthorizationStatus { get; set; }

        [JsonProperty(PropertyName = "displayProperties")]
        public object DisplayProperties { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "fullCardName")]
        public string FullCardName { get; set; }

        [JsonProperty(PropertyName = "isButton")]
        public bool IsButton { get; set; }

        [JsonProperty(PropertyName = "isCaptcha")]
        public bool IsCaptcha { get; set; }

        [JsonProperty(PropertyName = "isError")]
        public bool IsError { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "questionCode")]
        public int QuestionCode { get; set; }

        [JsonProperty(PropertyName = "questionContent")]
        public string QuestionContent { get; set; }

        [JsonProperty(PropertyName = "returnCode")]
        public int ReturnCode { get; set; }

        [JsonProperty(PropertyName = "returnMessage")]
        public object ReturnMessage { get; set; }

        [JsonProperty(PropertyName = "siteName")]
        public object SiteName { get; set; }

        [JsonProperty(PropertyName = "stage")]
        public object Stage { get; set; }

        [JsonProperty(PropertyName = "tablePageNum")]
        public int TablePageNum { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "userStatus")]
        public string UserStatus { get; set; }
    }
}