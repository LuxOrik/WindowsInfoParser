using System;
using System.Collections.Generic;
using System.IO;
using WindowsInfoGatherer.Collectors;
using CommandLine;
using CommandLine.Text;

namespace WindowsInfoGatherer.Option
{
    [Verb("export", HelpText = "Answer a call in the specified folder.")]
    internal class ExportOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Folder of the call to export.")]
        public string FolderPath { get; set; }
        [Option('o', "out", Required = true, HelpText = "File to export to.")]
        public string OutPath { get; set; }
        [Option('t', "type", Required = true, HelpText = "Type to export to. " + nameof(ExportType.Csv) + " or " + nameof(ExportType.Json))]
        public ExportType ExportType { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples => new[]
        {
            new Example(
                "Export a call to a CSV file",
                new ExportOptions
                {
                    FolderPath = "D:\\test\\Calls\\2018-02-21",
                    ExportType = ExportType.Csv,
                    OutPath = "D:\\test\\Calls\\2018-02-21 Report.csv"
                })
        };

        public void Execute()
        {
            switch (ExportType)
            {
                case ExportType.Csv:
                    CollectorToCsv.Collect(new DirectoryInfo(FolderPath), OutPath);
                    break;
                case ExportType.Json:
                    CollectorToJson.Collect(new DirectoryInfo(FolderPath), OutPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal enum ExportType
    {
        Csv,
        Json
    }
}
