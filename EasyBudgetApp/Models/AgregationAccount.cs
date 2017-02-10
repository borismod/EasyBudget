using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider;

namespace EasyBudgetApp.Models
{
    public class AgregationAccount : ICustomAccount
    {
        public VendorId VendorId { get; set; }
        public long AccountId { get; set; }
        public string UserId { get; }
        public string Label { get; set; }
        public double Balance { get; set; }
        public string Description { get; set; }
    }
}
