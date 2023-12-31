using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Plugins.Cache;

namespace HappyCRappy.SerializerLib;

public static class ModSerialization
{
    public static string SerializeYaml(SkyrimMod mod)
    {
        string result = string.Empty;
        using (MemoryStream stream = new MemoryStream())
        {
            // Perform operations on the stream
            Task.Run(() => MutagenYamlConverter.Instance.Serialize(mod, stream));

            // Convert the stream to a string
            result = Encoding.UTF8.GetString(stream.ToArray());
        }

        return result;
    }


    public static string SerializeJSON(SkyrimMod mod)
    {
        string result = string.Empty;
        using (MemoryStream stream = new MemoryStream())
        {
            // Perform operations on the stream
            Task.Run(() => MutagenJsonConverter.Instance.Serialize(mod, stream));

            // Convert the stream to a string
            result = Encoding.UTF8.GetString(stream.ToArray());

        }

        return result;
    }
}
