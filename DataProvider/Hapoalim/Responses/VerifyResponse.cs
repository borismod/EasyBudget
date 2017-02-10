using Newtonsoft.Json;

namespace DataProvider.Hapoalim.Responses
{
  public class VerifyResponse
  {
    public string Flow { get; set; }
    public string State { get; set; }
    public string Target { get; set; }
    public string Referrer { get; set; }
    public ResultResponse Result { get; set; }
    public ErrorResponse Error { get; set; }

    public class ResultResponse
    {
      public bool LastTry { get; set; }
      public string PasswordType { get; set; }
      public string ExpiredDate { get; set; }
      public bool Callme { get; set; }
    }

    public class ErrorResponse
    {
      [JsonProperty("errCode")]
      public string ErrorCode { get; set; }

      [JsonProperty("errDesc")]
      public string ErrorDescription { get; set; }
    }
  }
}
