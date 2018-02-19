using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace WindowsInfoParser.Option
{
    [Verb("create", HelpText = "Create a new call.")]
    internal class CreateOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder of the calls")]
        public string FolderPath { get; set; }

        [Option('d', "Date", HelpText = "Date at which the call will be valid. Clients will only start answering on this day and thereafter. Default to today")]
        public string ScheduledDate { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples => new[]
        {
            new Example("Create a new call starting today",
                new CreateOptions {FolderPath = "D:\\test\\Calls\\", ScheduledDate = DateTime.Today.ToString(CallFolderManager.Format)}),
            new Example("Create a new call starting in a week",
                new CreateOptions {FolderPath = "D:\\test\\Calls\\", ScheduledDate = DateTime.Today.AddDays(7).ToString(CallFolderManager.Format)})
        };
    }

    internal enum CreateResult
    {
        AlreadyExists,
        Ok
    }
}
