using System;
using System.Collections.Generic;

namespace SerializationLab
{
    public interface ICollectionSerializable
    {
        void Deserialize(List<object> jsonArray);

        List<object> GetObjects();
    }
}
