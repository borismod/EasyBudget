using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBudgetApp.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<long> Suppliers { get; private set; }

        public Category()
        {
            Suppliers = new List<long>();
        }

        public Category(Guid id, string name) : this()
        {
            Id = id;
            Name = name;
        }
    }
}
