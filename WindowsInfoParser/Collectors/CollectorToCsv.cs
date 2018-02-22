using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;

namespace WindowsInfoGatherer.Collectors
{
    public static class CollectorToCsv
    {
        public static void Collect(DirectoryInfo callDirectory, string outPath, int? minVersion = null)
        {
            var allAnswersQuery = CollectorHelpers.CollectCallToCsv(callDirectory);
            if (minVersion.HasValue)
                allAnswersQuery = allAnswersQuery.Where(pc => pc.DefinitionVersion >= minVersion.Value);

            var allAnswers = allAnswersQuery.ToList();
            using (var cW = new CsvWriter(new StreamWriter(outPath), false))
            {
                cW.Configuration.RegisterClassMap<SimplePcCsvMap>();
                cW.WriteRecords(allAnswers);
            }
            
        }
    }

    public sealed class SimplePcCsvMap : ClassMap<SimplePC>
    {
        public SimplePcCsvMap()
        {
            Map(pc => pc.DefinitionVersion).Name("Version");
            Map(pc => pc.Name).Name("Nom");
            Map(pc => pc.Domain).Name("Domaine");
            Map(pc => pc.Manufacturer).Name("Fabricant");
            Map(pc => pc.Model).Name("Modèle");
            Map(pc => pc.OSInstallDate).Name("Date d'installation");
            Map(pc => pc.OSName).Name("OS");
            Map(pc => pc.OSVersion).Name("OS Version");
            Map(pc => pc.TotalPhysicalMemory).Name("Mémoire");
            Map(pc => pc.AccessMacAddress).Name("MAC");
            Map(pc => pc.AccessIpAddresses).Name("IP");
        }
    } 


}
