using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataProvider.Amex.Requests
{
    public class ValidateIdBody
    {
        [JsonProperty(PropertyName = "applicationSource")]
        public string ApplicationSource { get; set; }

        [JsonProperty(PropertyName = "cardSuffix")]
        public string CardSuffix { get; set; }

        [JsonProperty(PropertyName = "checkLevel")]
        public string CheckLevel { get; set; }

        [JsonProperty(PropertyName = "companyCode")]
        public string CompanyCode { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "idType")]
        public string IdType { get; set; }

    }
}
