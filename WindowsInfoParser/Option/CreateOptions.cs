using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace WindowsInfoGatherer.Option
{
    [Verb("create", HelpText = "Create a new call.")]
    internal class CreateOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder of the calls")]
        public string FolderPath { get; set; }

        [Option('d', "Date", HelpText = "Date at which the call will be valid. Clients will only start answering on this day and thereafter. Default to today")]
        public string ScheduledDate { get; set; }

        private DateTime ResultingDate => ScheduledDate == null || !DateTime.TryParse(ScheduledDate, out var date)
            ? DateTime.Today
            : date;

        internal DateTime? UsedDate; // To avoid midnight error

        [Usage]
        public static IEnumerable<Example> Examples => new[]
        {
            new Example("Create a new call starting today",
                new CreateOptions {FolderPath = "D:\\test\\Calls\\", ScheduledDate = DateTime.Today.ToString(CallFolderManager.Format)}),
            new Example("Create a new call starting in a week",
                new CreateOptions {FolderPath = "D:\\test\\Calls\\", ScheduledDate = DateTime.Today.AddDays(7).ToString(CallFolderManager.Format)})
        };

        public CreateResult Execute() => CallFolderManager.CreateNewDefinitionCall(FolderPath, (UsedDate = ResultingDate).Value);
    }

    internal enum CreateResult
    {
        AlreadyExists,
        Ok
    }
}
