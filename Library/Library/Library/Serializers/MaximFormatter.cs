using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Reflection;

namespace Library
{
    public class MaximFormatter
    {
        private int spaceCount = 0;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private List<MaximFormatterObject> mainObjectList = new List<MaximFormatterObject>();
        private int objectId = 1;

        public void Serialize(Stream stream, object obj)
        {
            mainObjectList.Clear();
            streamWriter = new StreamWriter(stream);
            WriteObject(obj, 0, 0);
            streamWriter.Flush();
        }

        public object Deserialize(Stream stream)
        {
            using (streamReader = new StreamReader(stream))
            {
                StringBuilder text = new StringBuilder(streamReader.ReadToEnd());
                return ReadObject(text);
            }
        }

        private object ReadObject(StringBuilder text)
        {
            int openIndex = 0;
            int closeIndex = 0;
            GetBracketIndexes(text, '{', '}', ref openIndex, ref closeIndex);
            text = CutText(text, openIndex, closeIndex);

            MaximFormatterObject mfObj = new MaximFormatterObject();
            mfObj.Obj = null;
            int id;
            Type type;
            for (int i = 0; i < text.Length; i++)
            {
                // "$
                if ((text[i] == '"') && (text[i + 1] == '$'))
                {
                    i += 2;
                    // "$i
                    if (text[i] == 'i')
                    {
                        i += 6;
                        string stringId = "";
                        while (text[i] != '"')
                        {
                            stringId += text[i];
                            i++;
                        }
                        id = int.Parse(stringId);
                        mfObj.Id = id;
                    }
                    // "$t
                    else if (text[i] == 't')
                    {
                        i += 8;
                        string typeName = "";
                        while (text[i] != '"')
                        {
                            typeName += text[i];
                            i++;
                        }
                        type = Type.GetType(typeName);
                        mfObj.Obj = CreateObject(type);
                        mainObjectList.Add(mfObj);
                    }
                    // "$v
                    else if (text[i] == 'v')
                    {
                        IList list = (IList)mfObj.Obj;
                        GetBracketIndexes(text, '{', '}', ref openIndex, ref closeIndex);
                        StringBuilder elements = CutText(text, openIndex, closeIndex);
                        while (!IsEmpty(elements))
                        {
                            GetBracketIndexes(elements, '{', '}', ref openIndex, ref closeIndex);
                            StringBuilder element = CutText(elements, openIndex - 1, closeIndex + 1);
                            var listElement = ReadObject(element);
                            list.Add(listElement);
                            elements.Remove(0, closeIndex + 1);
                        }
                        mfObj.Obj = list;
                        break;
                    }
                    // "$r
                    else if (text[i] == 'r')
                    {
                        string stringId = "";
                        i += 7;
                        while (text[i] != '"')
                        {
                            stringId += text[i];
                            i++;
                        }
                        id = int.Parse(stringId);
                        return FindObject(id);
                    }
                }
                else if ((text[i] == '"') && (text[i + 1] != '$'))
                {
                    i++;
                    string propertyName = "";
                    while (text[i] != '"')
                    {
                        propertyName += text[i];
                        i++;
                    }
                    i += 4;
                    object objPropertyValue = null;
                    string strPropertyValue = "";
                    if (text[i - 1] == '{')
                    {
                        i -= 1;
                        int difference = 0;
                        while (true)
                        {
                            if (text[i] == '{')
                            {
                                if (difference == 0)
                                {
                                    openIndex = i;
                                }
                                difference++;
                            }
                            else if (text[i] == '}')
                            {
                                difference--;
                                if (difference == 0)
                                {
                                    closeIndex = i;
                                    break;
                                }
                            }
                            i++;
                        }
                        StringBuilder sb = CutText(text, openIndex - 1, closeIndex + 1);
                        objPropertyValue = ReadObject(sb);
                    }
                    else
                    {
                        while (text[i] != '"')
                        {
                            strPropertyValue += text[i];
                            i++;
                        }
                    }
                    foreach (PropertyInfo property in mfObj.Obj.GetType().GetProperties())
                    {
                        if (property.Name == propertyName)
                        {
                            if (property.PropertyType == typeof(int))
                            {
                                property.SetValue(mfObj.Obj, int.Parse(strPropertyValue));
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(mfObj.Obj, strPropertyValue);
                            }
                            else if (property.PropertyType == typeof(float))
                            {
                                property.SetValue(mfObj.Obj, float.Parse(strPropertyValue));
                            }
                            else if (property.PropertyType.IsEnum)
                            {
                                property.SetValue(mfObj.Obj, Enum.Parse(property.PropertyType, strPropertyValue));
                            }
                            else
                            {
                                try
                                {
                                    property.SetValue(mfObj.Obj, objPropertyValue);
                                }
                                catch
                                {
                                    //MessageBox.Show(mfObj.Obj.GetType().ToString() + "-" + property.PropertyType.ToString() + "-" + objPropertyValue.ToString());
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return mfObj.Obj;
        }

        private object CreateObject(Type type)
        {
            return Activator.CreateInstance(type);
        }

        private void GetBracketIndexes(StringBuilder data, char openBracket, char closeBracket, ref int openIndex, ref int closeIndex)
        {
            int difference = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == openBracket)
                {
                    if (difference == 0)
                    {
                        openIndex = i;
                    }
                    difference++;
                }
                else if (data[i] == closeBracket)
                {
                    difference--;
                    if (difference == 0)
                    {
                        closeIndex = i;
                        return;
                    }
                }
            }
        }
        private StringBuilder CutText(StringBuilder text, int startIndex, int closeIndex)
        {
            StringBuilder result = new StringBuilder("");
            for (int i = startIndex + 1; i < closeIndex; i++)
            {
                result.Append(text[i]);
            }
            return result;
        }

        private bool IsEmpty(StringBuilder text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if ((text[i] != ' ') && (text[i] != '\n'))
                {
                    return false;
                }
                else if (i == text.Length - 1)
                {
                    break;
                }
            }
            return true;
        }

        private void WriteObject(object obj, int currIndex, int maxIndex)
        {
            Type objectType = obj.GetType();
            Write("{\n");
            spaceCount += 2;
            WriteSpaces();

            if (!IsAlreadyExist(obj))
            {
                if (!IsBaseType(objectType))
                {
                    if (objectType.IsGenericType)
                    {
                        MaximFormatterObject mfObject = new MaximFormatterObject(obj, objectId);
                        mainObjectList.Add(mfObject);
                        Write($"\"$id\": \"{objectId.ToString()}\",\n");
                        objectId++;
                        WriteSpaces();
                        Write($"\"$type\": \"{objectType.ToString()}\",\n");

                        WriteSpaces();
                        Write($"\"$values\": {{\n");
                        spaceCount += 2;
                        WriteSpaces();

                        var objectList = (IList)obj;
                        int index = 0;
                        foreach (object listElement in objectList)
                        {
                            WriteObject(listElement, index, objectList.Count - 1);
                            if (index != objectList.Count - 1)
                                WriteSpaces();
                            index++;
                        }

                        spaceCount -= 2;
                        WriteSpaces();
                        Write("}\n");
                    }
                    else
                    {
                        MaximFormatterObject mfObject = new MaximFormatterObject(obj, objectId);
                        mainObjectList.Add(mfObject);
                        Write($"\"$id\": \"{objectId.ToString()}\",\n");
                        objectId++;
                        WriteSpaces();
                        Write($"\"$type\": \"{objectType.ToString()}\",\n");

                        PropertyInfo[] properties = objectType.GetProperties();
                        int index = 0;
                        foreach (PropertyInfo property in properties)
                        {
                            if (!IsBaseType(property.PropertyType))
                            {
                                if (property.PropertyType.IsEnum)
                                {
                                    WriteSpaces();
                                    Write($"\"{property.Name}\": \"{property.GetValue(obj)}\"\n");
                                }
                                else
                                {
                                    WriteSpaces();
                                    Write($"\"{property.Name}\": ");
                                    var someObject = (object)property;
                                    WriteObject(property.GetValue(obj), index, properties.Length - 1);
                                }
                            }
                            else
                            {
                                WriteSpaces();
                                Write($"\"{property.Name}\": \"{property.GetValue(obj).ToString()}\"");
                                if (index != properties.Length - 1)
                                {
                                    Write(",");
                                }
                                Write("\n");
                            }
                            index++;
                        }
                    }
                }
            }
            else
            {
                int id = FindId(obj);
                Write($"\"$ref\": \"{id.ToString()}\"\n");
            }

            spaceCount -= 2;
            WriteSpaces();
            Write("}");
            if (currIndex < maxIndex)
            {
                Write(",");
            }
            Write("\n");
        }

        private void WriteSpaces()
        {
            for (int i = 0; i < spaceCount; i++)
            {
                streamWriter.Write(" ");
            }
        }

        private int FindId(object obj)
        {
            foreach (MaximFormatterObject mfObject in mainObjectList)
            {
                if (mfObject.Obj == obj)
                {
                    return mfObject.Id;
                }
            }
            return 0;
        }

        private object FindObject(int id)
        {
            foreach (MaximFormatterObject mfObj in mainObjectList)
            {
                if (mfObj.Id == id)
                {
                    return mfObj.Obj;
                }
            }
            return null;
        }

        private void Write(string str)
        {
            streamWriter.Write(str);
        }

        private bool IsAlreadyExist(object obj)
        {
            foreach (MaximFormatterObject michaelFormatterObject in mainObjectList)
            {
                if (michaelFormatterObject.Obj == obj)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBaseType(Type type)
        {
            if (type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
                type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) ||
                type == typeof(ulong) || type == typeof(float) || type == typeof(double) || type == typeof(decimal) ||
                type == typeof(char) || type == typeof(string))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
