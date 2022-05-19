using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializationLab
{
    public class JsonSerializer: ISerializer
    {
        private class ObjectParseInfo
        {
            public int? NameStart { get; set; }

            public int? NameStop { get; set; }

            public int? ValueStart { get; set; }

            public int? ValueStop { get; set; }

            public string PropName { get; set; }

            public string PropValue { get; set; }


            public ObjectParseInfo()
            {
                Clear();
            }


            public void Clear()
            {
                NameStart = null;
                NameStop = null;
                ValueStart = null;
                ValueStop = null;
                PropName = null;
                PropValue = null;
            }

            public void Update(string str, int pos)
            {
                if (NameStart == null)
                    NameStart = pos + 1;
                else if (NameStop == null)
                {
                    NameStop = pos - 1;
                    PropName = str.Substring((int)NameStart, (int)NameStop - (int)NameStart + 1);
                }
                else if (ValueStart == null)
                    ValueStart = pos + 1;
                else if (ValueStop == null)
                {
                    ValueStop = pos - 1;
                    PropValue = str.Substring((int)ValueStart, (int)ValueStop - (int)ValueStart + 1);
                }
            }
        }

        private class ArrayParseInfo
        {
            public int? ItemStart { get; set; }

            public int? ItemStop { get; set; }

            public string ItemValue { get; set; }


            public ArrayParseInfo()
            {
                Clear();
            }


            public void Clear()
            {
                ItemStart = null;
                ItemStop = null;
                ItemValue = null;
            }

            public void Update(string str, int pos)
            {
                if (ItemStart == null)
                    ItemStart = pos + 1;
                else if (ItemStop == null)
                {
                    ItemStop = pos - 1;
                    ItemValue = str.Substring((int)ItemStart, (int)ItemStop - (int)ItemStart + 1);
                }
            }
        }


        private const string Offset = "   ";

        private const string TypePropStr = "$type";


        public JsonSerializer()
        {
            
        }


        string ISerializer.Serialize(object obj)
        {
            if (obj is ICollectionSerializable)
                return SerializeCollection((obj as ICollectionSerializable), 0).ToString();
            else
                return SerializeObject(obj, 0).ToString();
        }

        private StringBuilder SerializeObject(object obj, int offset)
        {
            if (obj is ICustomSerializable)
            {
                StringBuilder tmp = (obj as ICustomSerializable).Serialize();
                AppendOffset(tmp, string.Join("", Enumerable.Repeat(Offset, offset)));
                return tmp;
            }

            StringBuilder res = new StringBuilder();
            string offsetStr = string.Join("", Enumerable.Repeat(Offset, offset));
            string innerOffsetStr = string.Concat(offsetStr, Offset);

            res.Append(offsetStr);
            res.Append("{\n");

            StringBuilder typeRecord = CreateTypeRecord(obj.GetType());
            res.Append(innerOffsetStr);
            res.Append(typeRecord);
            res.Append(',');

            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (var item in props)
            {
                res.Append("\n");
                res.Append(innerOffsetStr);
                res.AppendFormat("\"{0}\" : ", item.Name);

                Type type = item.PropertyType;
                if (item.GetValue(obj) is ICollectionSerializable)
                {
                    res.Append("\n");
                    res.Append(SerializeCollection(item.GetValue(obj) as ICollectionSerializable, offset + 1));
                    res.Append(",\n");
                }
                else if ((!type.IsPrimitive) && (type != typeof(string)))
                {
                    res.Append("\n");
                    res.Append(SerializeObject(item.GetValue(obj), offset + 1));
                    res.Append(",\n");
                }
                else
                    res.AppendFormat("\"{0}\",", item.GetValue(obj));
            }

            if (res[res.Length - 1] == ',')
                res.Remove(res.Length - 1, 1);

            res.Append("\n");
            res.Append(offsetStr);
            res.Append("}");

            return res;
        }

        private StringBuilder SerializeCollection(ICollectionSerializable obj, int offset)
        {
            StringBuilder res = new StringBuilder();
            string offsetStr = string.Join("", Enumerable.Repeat(Offset, offset));
            string innerOffsetStr = string.Concat(offsetStr, Offset);

            res.Append(offsetStr);
            res.Append("[\n");

            StringBuilder typeRecord = CreateTypeRecord(obj.GetType());
            res.Append(innerOffsetStr);
            res.Append(typeRecord);
            res.Append(',');

            foreach (var item in obj.GetObjects())
            {
                res.Append("\n");
                res.Append(innerOffsetStr);

                Type type = (obj as ICollectionSerializable).GetType().GenericTypeArguments[0];

                if (item is ICollectionSerializable)
                {
                    res.Append("\n");
                    res.Append(SerializeCollection(item as ICollectionSerializable, offset + 1));
                    res.Append(",\n");
                }
                else if ((!type.IsPrimitive) && (type != typeof(string)))
                {
                    res.Append("\n");
                    res.Append(SerializeObject(item, offset + 1));
                    res.Append(",\n");
                }
                else
                    res.AppendFormat("\"{0}\",", item);
            }

            if (res[res.Length - 1] == ',')
                res.Remove(res.Length - 1, 1);

            res.Append("\n");
            res.Append(offsetStr);
            res.Append("]");

            return res;
        }

        object ISerializer.Deserialize(string str, Type type)
        {
            int pos = 0;
            if (type.GetInterface(nameof(ICollectionSerializable)) != null)
                return DeserializeArray(str, ref pos);
            else
                return DeserializeObject(str, ref pos);
        }

        private object DeserializeObject(string str, ref int pos)
        {
            Type newType = ParseTypeRecord(str, ref pos);

            if (newType is ICustomSerializable)
                return (newType as ICustomSerializable).Deserialize(str);

            ConstructorInfo info = newType.GetConstructor(Type.EmptyTypes);
            var res = info.Invoke(new object[0]);

            ObjectParseInfo pInfo = new ObjectParseInfo();

            while (pos < str.Length)
            {
                switch (str[pos])
                {
                    case '"':
                    {
                        pInfo.Update(str, pos);
                        if (pInfo.PropValue != null)
                        {
                            var prop = newType.GetProperty(pInfo.PropName);
                            prop.SetValue(res, Convert.ChangeType(pInfo.PropValue, prop.PropertyType), null);
                            pInfo.Clear();
                        }

                        break;
                    }

                    case '{':
                    {
                        if (pInfo.PropName != null)
                        {
                            var propObj = newType.GetProperty(pInfo.PropName);
                            var propObjValue = DeserializeObject(str, ref pos);
                            propObj.SetValue(res, propObjValue);
                            pInfo.Clear();
                        }

                        break;
                    }

                    case '}':
                    {
                        return res;
                    }

                    case '[':
                    {
                        if (pInfo.PropName != null)
                        {
                            var propObj = newType.GetProperty(pInfo.PropName);
                            var propObjValue = DeserializeArray(str, ref pos);
                            propObj.SetValue(res, propObjValue);
                            pInfo.Clear();
                        }
                        else
                            throw new Exception("Deserialization parsing error");

                        break;
                    }
                }

                ++pos;
            }

            throw new Exception("Deserialization error");
        }

        private object DeserializeArray(string str, ref int pos)
        {
            Type newType = ParseTypeRecord(str, ref pos);
            ConstructorInfo info = newType.GetConstructor(Type.EmptyTypes);
            var res = info.Invoke(new object[0]);

            if (res is ICollectionSerializable)
            {
                List<object> items = new List<object>();

                ArrayParseInfo pInfo = new ArrayParseInfo();

                while (pos < str.Length)
                {
                    switch (str[pos])
                    {
                        case '"':
                        {
                            pInfo.Update(str, pos);
                            if (pInfo.ItemValue != null)
                            {
                                //for primitives only
                                var t = Type.GetTypeCode((res as ICollectionSerializable).GetType().GenericTypeArguments[0]);
                                items.Add(Convert.ChangeType(pInfo.ItemValue, t));
                                pInfo.Clear();
                            }

                            break;
                        }

                        case ']':
                        {
                            (res as ICollectionSerializable).Deserialize(items);
                            return res;
                        }

                        case '{':
                        {
                            object newItem = DeserializeObject(str, ref pos);
                            items.Add(newItem);
                            pInfo.Clear();

                            break;
                        }

                        case '[':
                        {
                            if (pInfo.ItemValue != null)
                            {
                                object newItem = DeserializeArray(str, ref pos);
                                items.Add(newItem);
                                pInfo.Clear();
                            }

                            break;
                        }
                    }

                    ++pos;
                }

                throw new Exception("Deserialization error");
            }
            else
                return res;
        }

        private void AppendOffset(StringBuilder str, string offsetStr)
        {
            for (int i = str.Length - 2; i >= 0; --i)
                if (str[i] == '\n')
                    str.Insert(i + 1, offsetStr);
            str.Insert(0, offsetStr);
        }

        private StringBuilder CreateTypeRecord(Type type)
        {
            StringBuilder res = new StringBuilder();
            res.Append('"');
            res.Append(TypePropStr);
            res.Append("\" : \"");
            res.Append(type.ToString());
            res.Append('"');
            return res;
        }

        private Type ParseTypeRecord(string str, ref int pos)
        {
            ObjectParseInfo info = new ObjectParseInfo();

            while (pos < str.Length)
            {
                if (str[pos] == '"')
                {
                    info.Update(str, pos);
                    if ((info.PropName != null) && (info.PropName.Equals(TypePropStr)) && (info.PropValue != null))
                    {
                        ++pos;
                        return Type.GetType(info.PropValue);
                    }
                }

                ++pos;
            }

            return null;
        }
    }
}
