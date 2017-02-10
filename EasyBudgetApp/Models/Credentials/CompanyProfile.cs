using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EasyBudgetApp.Models.Credentials
{
  public class CompanyProfile
  {
    [JsonProperty("Name")]
    public string Name { get; set; }
    
    [JsonProperty("Credentials")]
    public IEnumerable<string> Credentials{ get; set; }

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
      if (Credentials == null)
      {
        Credentials = Enumerable.Empty<string>();
      }
    }
  }
}
