using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsInfoGatherer.Collectors
{
    public sealed class CollectorToJson
    {
        public static void Collect(DirectoryInfo callDirectory, string outPath, int? minVersion = null)
        {
            var allAnswersQuery = CollectorHelpers.CollectCalls(callDirectory);
            if (minVersion.HasValue)
                allAnswersQuery = allAnswersQuery.Where(pc => pc.DefinitionVersion >= minVersion.Value);

            var allAnswers = allAnswersQuery.ToList();
            using (var writer = new JsonTextWriter(File.CreateText(outPath)))
            {
                SimplePC.Serializer.Serialize(writer, allAnswers);
            }
        }
    }
}
