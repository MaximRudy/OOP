using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Library
{
    public interface IBaseSerializer
    {
        void Serialize(Stream streamToSerialize, object objectToSerialize);

        object Deserialize(Stream streamToDeserialize);

        string GetExtention();
    }
}
