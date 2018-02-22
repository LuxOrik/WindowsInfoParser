using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsInfoGatherer.Collectors
{
    public static class CollectorHelpers
    {
        public static IEnumerable<SimplePC> CollectCallToCsv(DirectoryInfo directory)
            => CollectCallToCsv(directory.FullName);

        public static IEnumerable<SimplePC> CollectCallToCsv(string path)
            => Directory.EnumerateFiles(path, "*.json").Select(ReadFile).OrderBy(pc => pc.Name);
        
        private static SimplePC ReadFile(string filePath)
        {
            using (var jR = new JsonTextReader(File.OpenText(filePath)))
            {
                return SimplePC.Serializer.Deserialize<SimplePC>(jR);
            }
        }
    }
}
