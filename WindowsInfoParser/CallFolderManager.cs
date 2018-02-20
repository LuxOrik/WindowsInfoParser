using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInfoParser.Option;

namespace WindowsInfoParser
{
    internal static class CallFolderManager
    {
        internal const string Format = "yyyy-MM-dd";

        public static bool CheckPathAvailability(string path, TimeSpan timeout)
        {
            var t = Task.Run(() => Directory.Exists(path));
            return t.Wait(timeout) && t.Result;
        }

        public static CreateResult CreateNewDefinitionCall(string path, DateTime date)
        {
            if (!CheckPathAvailability(path, TimeSpan.FromSeconds(1)))
                throw new DirectoryNotFoundException("The directory doesn't seem to be available.");

            var directoryName = date.ToString(Format);
            var directoryFullPath = Path.Combine(path, directoryName);

            if (Directory.Exists(directoryFullPath))
                return CreateResult.AlreadyExists;

            Directory.CreateDirectory(directoryFullPath);
            return CreateResult.Ok;
        }

        public static IEnumerable<(DirectoryInfo Folder, DateTime Date)> GetAllDefinitionCalls(string path)
        {
            if (!CheckPathAvailability(path, TimeSpan.FromSeconds(1)))
                throw new DirectoryNotFoundException("The directory doesn't seem to be available.");
            
            foreach (var tuple in Directory.EnumerateDirectories(path)
                                           .Select(dir => new DirectoryInfo(dir))
                                           .Select(
                                               dirInfo => (dirInfo,
                                                   DateTime.TryParse(dirInfo.Name, out var date)
                                                       ? date
                                                       : DateTime.MinValue))
                                           .Where(tuple => tuple.Item2 != DateTime.MinValue)
                                           .OrderByDescending(tuple => tuple.Item2))
            {
                yield return tuple;
            }
        }

        public static (DirectoryInfo Folder, DateTime Date) GetLastDefinitionCall(string path) => GetAllDefinitionCalls(path).Last();
    }
}
