using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider;
using DataProvider.Interfaces;
using Newtonsoft.Json;

namespace EasyBudgetApp.Models.Credentials
{
    public class UserAccountProfile
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Details")]
        public string Details { get; set; }

        [JsonProperty("IsEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("VendorName")]
        public String InstitutionName { get; set; }

        public String UserId { get; set; }

        public IAccount ToAccount()
        {
            return new BasicAccount(Name, Id, IsEnabled, UserId);
        }

        public override bool Equals(object obj)
        {
            var profile = obj as UserAccountProfile;
            if (profile == null)
            {
                return false;
            }

            return Equals(profile);
        }

        public bool Equals(UserAccountProfile profile)
        {
            return Id == profile?.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
