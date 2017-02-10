using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProvider;
using DataProvider.Interfaces;

namespace EasyBudgetApp.Models
{
  public interface ICustomAccount : IAccount
  {
    String Description { get; set; }
  }
}
