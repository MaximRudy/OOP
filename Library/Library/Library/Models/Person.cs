using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    public enum Gender
    {
        [Description("Male")]
        Male,
        [Description("Female")]
        Female
    }

    [Serializable]
    public abstract class Person : IBaseObject
    {
        [Description("Name")]
        public string Name { get; set; }

        [Description("Surname")]
        public string Surname { get; set; }

        [Description("Patronymic")]
        public string Patronymic { get; set; }

        [Description("Gender")]
        public Gender Gender { get; set; }

        public string GetId()
        {
            return Name;
        }
    }
}
