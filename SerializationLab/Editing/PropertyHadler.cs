using System;
using System.Collections.Generic;
using System.Reflection;

namespace SerializationLab
{
    public class PropertyHadler: IPropertyHandler
    {
        public PropertyHadler()
        {

        }


        public List<ExtendedPropertyInfo> GetProperties(object obj)
        {
            List<ExtendedPropertyInfo> res = new List<ExtendedPropertyInfo>();
            foreach (var prop in obj.GetType().GetProperties())
                res.Add(new ExtendedPropertyInfo(prop, prop.GetValue(obj)));

            return res;
        }

        public bool TrySetProperty(object obj, string value, PropertyInfo prop)
        {
            try
            {
                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType), null);
                return true;
            }
            catch(FormatException e)
            {
                return false;
            }
        }

        public object CreateObject(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                if (info != null)
                    return info.Invoke(new object[0]);
            }

            return null;
        }

        public PropertyInfo GetProperty(object obj, string name)
        {
            return obj.GetType().GetProperty(name);
        }

        public PropertyInfo GetProperty(object obj, int index)
        {
            return GetProperties(obj)[index].Property;
        }
    }
}
