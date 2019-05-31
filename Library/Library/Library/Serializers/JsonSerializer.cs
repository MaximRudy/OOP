using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Library
{
    [Description("Json serializer")]
    public class JsonSerializer : IBaseSerializer
    {
        public void Serialize(Stream streamToSerialize, object objectToSerialize)
        {
            string jsonString = JsonConvert.SerializeObject(
                objectToSerialize,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                }
            );
            streamToSerialize.Seek(0, SeekOrigin.Begin);
            StreamWriter writer = new StreamWriter(streamToSerialize);
            writer.WriteLine(jsonString);
            writer.Flush();
        }

        public object Deserialize(Stream streamToDeserialize)
        {
            streamToDeserialize.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(streamToDeserialize);
            string jsonString = reader.ReadToEnd();
            reader.Close();
            object deserializedObject = JsonConvert.DeserializeObject(
                jsonString,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
            return deserializedObject;
        }

        public string GetExtention()
        {
            return "json";
        }
    }
}
