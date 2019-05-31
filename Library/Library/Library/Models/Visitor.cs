using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    [Description("Visitor")]
    public class Visitor : Person
    {
        [Description("Card number")]
        public int CardNumber { get; set; }
    }
}
