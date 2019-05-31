using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MaximFormatterObject
    {
        public object Obj { get; set; }
        public int Id { get; set; }

        public MaximFormatterObject(object obj, int id)
        {
            Obj = obj;
            Id = id;
        }

        public MaximFormatterObject()
        {

        }
    }
}
