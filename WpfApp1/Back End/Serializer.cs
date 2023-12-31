using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Cache;
using HappyCRappy.SerializerLib;

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
            case SerializationType.JSON: output = ModSerialization.SerializeJSON(tempMod); break;
            case SerializationType.YAML: output = ModSerialization.SerializeYaml(tempMod); break;
        }
        return output;
    }
}
