using System.Reflection;

namespace SerializationLab
{
    public class ExtendedPropertyInfo
    {
        public PropertyInfo Property { get; }

        public object PropertyValue { get; }


        public ExtendedPropertyInfo(PropertyInfo property, object value)
        {
            Property = property;
            PropertyValue = value;
        }
    }
}
