using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsInfoParser
{
    internal static class DefinitionCallHandler
    {
        public const string CallDirectoryName = "Calls";
        public static void CreateNewDefinitionCall(DateTime? date = null)
        {
            date = date ?? DateTime.Today;
            var directoryName = date.Value.ToString("yyyy-MM-dd");
            var directoryPath = Path.Combine(Environment.CurrentDirectory, CallDirectoryName);
            var directoryFullPath = Path.Combine(directoryPath, directoryName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (Directory.Exists(directoryFullPath))
                throw new IOException($"A directory for a call already exists for the specified day ({date}).");

            Directory.CreateDirectory(directoryFullPath);
        }

        public static IEnumerable<(DirectoryInfo, DateTime)> GetAllDefinitionCalls()
        {
            var directoryPath = Path.Combine(Environment.CurrentDirectory, CallDirectoryName);
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

        public static (DirectoryInfo, DateTime) GetLastDefinitionCall() => GetAllDefinitionCalls().Last();
    }
}
