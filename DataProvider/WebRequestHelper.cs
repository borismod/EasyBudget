using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
  public static class WebRequestHelper
  {
    public static bool TryAddCookie(this WebRequest webRequest, Cookie cookie)
    {
      var httpRequest = webRequest as HttpWebRequest;
      if (httpRequest == null)
      {
        return false;
      }

      if (httpRequest.CookieContainer == null)
      {
        httpRequest.CookieContainer = new CookieContainer();
      }

      try
      {
        httpRequest.CookieContainer.Add(cookie);
      }
      catch (Exception exp)
      {
        Console.WriteLine(exp);
        return false;
      }
      
      return true;
    }
  }
}
