using CommandLine;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class IOFunctions
{
    public enum PathType
    {
        File,
        Directory
    }
    public static dynamic CreateDirectoryIfNeeded(string path, PathType type)
    {
        if (type == PathType.File)
        {
            FileInfo file = new FileInfo(path);
            file.Directory?.Create(); // If the directory already exists, this method does nothing.
            return file;
        }
        else
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            directory.Create();
            return directory;
        }
    }

    public static async Task WriteTextFile(string path, string contents)
    {
        var file = CreateDirectoryIfNeeded(path, PathType.File);

        try
        {
            await File.WriteAllTextAsync(file.FullName, contents);
        }
        catch (Exception e)
        {
            //logger.LogError("Could not create file at " + path + "because: " + Environment.NewLine + ExceptionLogger.GetExceptionStack(e));
        }
    }
    public static async Task WriteTextFile(string path, List<string> contents)
    {
        await WriteTextFile(path, string.Join(Environment.NewLine, contents));
    }

    public static async Task WriteTextFileStatic(string path, string contents)
    {
        var file = CreateDirectoryIfNeeded(path, PathType.File);

        try
        {
            await File.WriteAllTextAsync(file.FullName, contents);
        }
        catch (Exception e)
        {
            //var error = ExceptionLogger.GetExceptionStack(e);
            //MessageWindow.DisplayNotificationOK("Could not save text file", "Error: could not save text file to " + path + ". Exception: " + Environment.NewLine + error);
        }
    }

    public bool TryDeleteFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                //logger.LogErrorWithStatusUpdate("Could not delete file - see log", ErrorType.Warning);
                //string error = ExceptionLogger.GetExceptionStack(e);
                //logger.LogMessage("Could not delete file: " + path + Environment.NewLine + error);
                return false;
            }
        }
        return true;
    }

    public bool TryDeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                //logger.LogErrorWithStatusUpdate("Could not delete directory - see log", ErrorType.Warning);
                //string error = ExceptionLogger.GetExceptionStack(e);
                //logger.LogMessage("Could not delete directory: " + path + Environment.NewLine + error);
                return false;
            }
        }
        return true;
    }
}
