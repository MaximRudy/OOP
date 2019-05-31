using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    public enum BookType
    {
        [Description("Fantacy")]
        Fantasy,
        [Description("Romance")]
        Romance,
        [Description("Thriller")]
        Thriller,
        [Description("Mystery")]
        Mystery,
        [Description("Detective")]
        Detective,
        [Description("Horror")]
        Horror
    }

    [Description("Book")]
    public class Book : IBaseObject
    {
        [Description("Id")]
        public int Id { get; set; }

        [Description("Name")]
        public string Name { get; set; }

        [Description("Author")]
        public string Author { get; set; }

        [Description("Page count")]
        public int PageCount { get; set; }

        [Description("Genre")]
        public BookType BookType { get; set; }

        public string GetId()
        {
            return Id.ToString();
        }
    }
}
