using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    [Serializable]
    [Description("Employee")]
    public class Employee : Person
    {
        [Description("Salary")]
        public float Salary { get; set; }
    }
}
