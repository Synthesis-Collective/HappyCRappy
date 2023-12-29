using Mutagen.Bethesda.Skyrim;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mutagen.Bethesda.Json;

namespace HappyCRappy;

public class JSONhandler<T>
{
    public static JsonSerializerSettings GetSynthEBDJSONSettings()
    {
        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.AddMutagenConverters();
        jsonSettings.ObjectCreationHandling = ObjectCreationHandling.Replace;
        jsonSettings.Formatting = Formatting.Indented;
        jsonSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()); // https://stackoverflow.com/questions/2441290/javascriptserializer-json-serialization-of-enum-as-string

        return jsonSettings;
    }

    public static T? Deserialize(string jsonInputStr, out bool success, out string exception)
    {
        try
        {
            success = true;
            exception = "";
            JsonSerializerSettings serializerSettings = GetSynthEBDJSONSettings();
            if (serializerSettings == null)
            {
                throw new Exception("Cannot create Serializer settings");
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(jsonInputStr, serializerSettings);
            }
        }
        catch (Exception ex)
        {
            success = false;
            exception = ExceptionLogger.GetExceptionStack(ex);
            return default(T);
        }
    }

    public static T? LoadJSONFile(string loadLoc, out bool success, out string exception)
    {
        if (!File.Exists(loadLoc))
        {
            success = false;
            exception = "File " + loadLoc + " does not exist.";
            return default(T);
        }

        string contents = String.Empty;

        try
        {
            contents = File.ReadAllText(loadLoc);
        }
        catch (Exception ex)
        {
            success = false;
            exception = ExceptionLogger.GetExceptionStack(ex);
            return default(T);
        }

        if (contents == null || contents.IsNullOrWhitespace())
        {
            success = false;
            exception = "File " + loadLoc + " is empty.";
            return default(T);
        }

        return Deserialize(contents, out success, out exception);
    }

    public static string Serialize(T input, out bool success, out string exception)
    {
        try
        {
            success = true;
            exception = "";
            return JsonConvert.SerializeObject(input, Formatting.Indented, GetSynthEBDJSONSettings());
        }
        catch (Exception ex)
        {
            exception = ex.Message;
            success = false;
            return "";
        }
    }

    public static void SaveJSONFile(T input, string saveLoc, out bool success, out string exception)
    {
        try
        {
            IOFunctions.CreateDirectoryIfNeeded(saveLoc, IOFunctions.PathType.File);
            File.WriteAllText(saveLoc, Serialize(input, out success, out exception));
        }
        catch (Exception ex)
        {
            exception = ex.Message;
            success = false;
        }
    }
}
