using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace EasyBudgetApp.Models.Credentials
{
    public class UserProfile
    {
        [JsonProperty("Type")]
        public string InstitutionType { get; set; }

        [JsonProperty("Name")]
        public string InstitutionName { get; set; }

        [JsonProperty("Credentials")]
        public IDictionary<string, string> Credentials { get; set; }

        [JsonProperty("Accounts")]
        public IList<UserAccountProfile> Accounts { get; set; }

        [JsonProperty("UserId")]
        public string UserId { get; set; }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (Accounts == null)
            {
                Accounts = Enumerable.Empty<UserAccountProfile>().ToList();
            }

            if (Credentials == null)
            {
                Credentials = new Dictionary<string, string>();
            }
        }

        public override bool Equals(object obj)
        {
            var profile = obj as UserProfile;
            if (profile == null)
            {
                return false;
            }

            return Equals(profile);
        }

        public bool Equals(UserProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            var keysIntersect = Credentials.Keys.Intersect(profile.Credentials.Keys).ToList();
            var valuesIntersect = Credentials.Values.Intersect(profile.Credentials.Values).ToList();

            return InstitutionName.Equals(profile.InstitutionName, StringComparison.InvariantCultureIgnoreCase) &&
                   keysIntersect.Count == profile.Credentials.Keys.Count &&
                   valuesIntersect.Count == profile.Credentials.Values.Count;
        }

        public override int GetHashCode()
        {
            StringBuilder sb = new StringBuilder(InstitutionName);
            foreach (var data in Credentials.Values)
            {
                sb.Append(data);
            }

            return sb.GetHashCode();
        }
    }
}
