using System;
using System.Collections.Generic;

namespace SerializationLab
{
    public class SerializableList<T> : List<T>, ICollectionSerializable
    {
        void ICollectionSerializable.Deserialize(List<object> jsonArray)
        {
            foreach (dynamic item in jsonArray)
            {
                this.Add(item);
            }
        }

        List<object> ICollectionSerializable.GetObjects()
        {
            List<object> res = new List<object>();

            foreach (var item in this)
                res.Add(item);

            return res;
        }
    }
}
