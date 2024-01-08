using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HappyCRappy;

public class YAMLHandler<T>
{
    public static T? Deserialize(string yamlInputString, out bool success, out string exception)
    {
        try
        {
            success = true;
            exception = "";
            var deserializer = new DeserializerBuilder()
            //.WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            return deserializer.Deserialize<T>(yamlInputString);
        }
        catch (Exception ex)
        {
            success = false;
            exception = ExceptionLogger.GetExceptionStack(ex);
            return default(T);
        }
    }

    public static T? LoadYAMLFile(string loadLoc, out bool success, out string exception)
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
            var serializer = new SerializerBuilder()
            //.WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            return serializer.Serialize(input);
        }
        catch (Exception ex)
        {
            exception = ex.Message;
            success = false;
            return "";
        }
    }

    public static void SaveYAMLFile(T input, string saveLoc, out bool success, out string exception)
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
