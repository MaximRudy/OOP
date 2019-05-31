using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Library
{
    public enum MagazineType
    {
        [Description("Fashion")]
        Fashion,
        [Description("Games")]
        Games,
        [Description("Health")]
        Health,
        [Description("Sport")]
        Sport,
        [Description("Music")]
        Music,
        [Description("Comics")]
        Comics
    }

    [Description("Magazine")]
    public class Magazine : IBaseObject
    {
        [Description("Id")]
        public int Id { get; set; }

        [Description("Name")]
        public string Name { get; set; }

        [Description("Publisher")]
        public string Publisher { get; set; }

        [Description("Magazine type")]
        public MagazineType MagazineType { get; set; }

        public string GetId()
        {
            return Id.ToString();
        }
    }
}
