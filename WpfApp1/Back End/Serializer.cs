using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Serialization.Yaml;
using Mutagen.Bethesda.Serialization.Newtonsoft;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Serialization.Streams;
using Mutagen.Bethesda.Plugins.Cache;

namespace HappyCRappy;

public class Serializer
{
    public Serializer()
    {

    }

    public string SerializeRecord(IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext, SerializationType type)
    {
        string output = string.Empty;
        var tempMod = new SkyrimMod(new("temp.esp", Mutagen.Bethesda.Plugins.ModType.Plugin), SkyrimRelease.SkyrimSE);
        recordContext.GetOrAddAsOverride(tempMod);
        switch(type)
        {
            case SerializationType.JSON: output = SerializeJSON(tempMod); break;
            case SerializationType.YAML: output = SerializeYaml(tempMod); break;
        }
        return output;
    }

    public string SerializeYaml(SkyrimMod mod)
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


    public string SerializeJSON(SkyrimMod mod)
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

    private IGroup GetPatchRecordGroup(IMajorRecordGetter recordGetter, ISkyrimMod outputMod)
    {
        var getterType = LoquiRegistration.GetRegister(recordGetter.GetType()).GetterType;
        return outputMod.GetTopLevelGroup(getterType);
    }
}
