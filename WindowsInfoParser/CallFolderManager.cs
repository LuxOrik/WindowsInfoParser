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

        public static CreateResult CreateNewDefinitionCall(string path, DateTime date)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            var directoryName = date.ToString(Format);
            var directoryFullPath = Path.Combine(path, directoryName);

            if (Directory.Exists(directoryFullPath))
                return CreateResult.AlreadyExists;

            Directory.CreateDirectory(directoryFullPath);
            return CreateResult.Ok;
        }

        public static IEnumerable<(DirectoryInfo Folder, DateTime Date)> GetAllDefinitionCalls(string path)
        {
            var directoryPath = Path.Combine(Environment.CurrentDirectory, path);
            if (!Directory.Exists(directoryPath))
                yield break;
            foreach (var tuple in Directory.EnumerateDirectories(directoryPath)
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
