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
using Mutagen.Bethesda.Serialization.Customizations;
using System.IO.Abstractions;

namespace HappyCRappy.SerializerLib;

public static class ModSerialization
{
    public static (string, string) SerializeRecord(SkyrimMod placeholderMod, string type) // expects a dummy mod containing the single record to be serialized
    {
        (string, string) result = (string.Empty, string.Empty);

        string destinationFolderPath = Path.Combine("C:", "MutagenSerialization");
        var vfs = new System.IO.Abstractions.TestingHelpers.MockFileSystem();

        switch(type)
        {
            case "JSON": MutagenJsonConverter.Instance.Serialize(placeholderMod, destinationFolderPath, fileSystem: vfs).Wait(); break;
            case "YAML": MutagenYamlConverter.Instance.Serialize(placeholderMod, destinationFolderPath, fileSystem: vfs).Wait(); break;
        }

        var directories = vfs.Directory.EnumerateDirectories(destinationFolderPath).ToArray();
        if (directories != null && directories.Length == 1)
        {
            var dirName = Path.GetFileName(directories[0]);
            var files = vfs.Directory.EnumerateFiles(directories[0]).ToArray();
            if (files != null && files.Length == 1)
            {
                var text = vfs.File.ReadAllText(files[0]);
                result.Item1 = dirName;
                result.Item2 = text;
            }
        }

        return result;
    }

    public class CustomizeOverall : ICustomize
    {
        public void Customize(ICustomizationBuilder builder)
        {
            builder
                .FilePerRecord();
        }
    }
}
