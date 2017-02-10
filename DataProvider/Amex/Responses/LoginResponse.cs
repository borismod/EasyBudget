using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider.Amex.Responses
{
    public class LoginResponse
    {
        public object BankName { get; set; }
        public string FirstName { get; set; }
        public bool IsBank { get; set; }
        public object IsCaptcha { get; set; }
        public bool IsSeenPop { get; set; }
        public string LastLoginDate { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }

    }
}
