using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    [Description("Assortment")]
    public class Assortment : IBaseObject
    {
        [Description("Name")]
        public string Name { get; set; }

        [Description("Books")]
        public List<Book> Books { get; set; }

        [Description("Magazines")]
        public List<Magazine> Magazines { get; set; }

        public string GetId()
        {
            return Name;
        }
    }
}
