using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace HappyCRappy;

public class SerializationSwitcher
{
    public SerializationSwitcher()
    {

    }

    public string SwitchSerialization(string currentSerialization, SerializationType currentSerializationType, SerializationType newSerializationType, out bool success, out string exceptionStr)
    {
        success = true;
        exceptionStr = string.Empty;

        if (currentSerializationType == newSerializationType)
        {
            return currentSerialization;
        }

        ExpandoObject? deserialized = new();

        if(currentSerializationType == SerializationType.JSON)
        {
            deserialized = JSONhandler<ExpandoObject>.Deserialize(currentSerialization, out success, out exceptionStr);
        }
        else if (currentSerializationType == SerializationType.YAML)
        {
            deserialized = YAMLHandler<ExpandoObject>.Deserialize(currentSerialization, out success, out exceptionStr);
        }

        if(!success)
        {
            return currentSerialization;
        }
        else if (deserialized == null)
        {
            exceptionStr = "Deserialization failed";
            return currentSerialization;
        }

        string? output = string.Empty;
        if(newSerializationType == SerializationType.JSON)
        {
            output = JSONhandler<ExpandoObject>.Serialize(deserialized, out success, out exceptionStr);
        }
        else if (newSerializationType == SerializationType.YAML)
        {
            output = YAMLHandler<ExpandoObject>.Serialize(deserialized, out success, out exceptionStr);
        }

        if (!success)
        {
            return currentSerialization;
        }
        else if (output == null)
        {
            exceptionStr = "Serialization failed";
            return currentSerialization;
        }

        return output;
    }
}
