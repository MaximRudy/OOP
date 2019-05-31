using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace Library
{
    [Description("Binary serializer")]
    public class BinarySerializer : IBaseSerializer
    {
        BinaryFormatter formatter = new BinaryFormatter();

        public void Serialize(Stream streamToSerialize, object objectToSerialize)
        {
            streamToSerialize.Seek(0, SeekOrigin.Begin);
            formatter.Serialize(streamToSerialize, objectToSerialize);
        }

        public object Deserialize(Stream streamToDeserialize)
        {
            streamToDeserialize.Seek(0, SeekOrigin.Begin);
            return formatter.Deserialize(streamToDeserialize);
        }

        public string GetExtention()
        {
            return "binary";
        }
    }
}
