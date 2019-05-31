using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    [Description("Library")]
    public class Library : IBaseObject
    {
        [Description("Name")]
        public string Name { get; set; }

        [Description("Employees")]
        public List<Employee> Employees { get; set; }

        [Description("Visitors")]
        public List<Visitor> Visitors { get; set; }

        [Description("Assortment")]
        public Assortment Assortment { get; set; }

        public string GetId()
        {
            return Name;
        }
    }
}
