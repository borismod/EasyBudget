namespace DataProvider.Hapoalim.Responses
{
  public class InitLoginResponse
  {
    public string Flow { get; set; }
    public string State { get; set; }
    public string Target { get; set; }
    public string Referer { get; set; }
    public InitLoginResult Result { get; set; }
    public string Error { get; set; }

    public class InitLoginResult
    {
      public string Challenge { get; set; }
      public string Guid { get; set; }
    }
  }
}
