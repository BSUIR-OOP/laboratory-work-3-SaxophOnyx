using System.Text;

namespace SerializationLab
{
    public interface ICustomSerializable
    {
        StringBuilder Serialize();

        object Deserialize(string str);
    }
}
