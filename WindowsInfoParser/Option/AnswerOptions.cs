using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace WindowsInfoGatherer.Option
{
    [Verb("answer", HelpText = "Answer a call in the specified folder.")]
    internal class AnswerOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder of the calls")]
        public string FolderPath { get; set; }

        [Option('r', "repeat", HelpText = "Answer last call even if it was already answered.")]
        public bool Repeat { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples => new[] {new Example("Check call to answer", new AnswerOptions {FolderPath = "D:\\test\\Calls\\"})};

        public CallAnswer Execute()
        {
            var today = DateTime.Today;
            var lowerLimit = today - TimeSpan.FromDays(365 * 2);
            var (folder, date) = CallFolderManager.GetAllDefinitionCalls(FolderPath).FirstOrDefault(call => call.Date <= today);
            if (date < lowerLimit)
                return CallAnswer.NoCallToAnswerFound;

            var definition = SimplePC.GetCurrentMachineDefinition();
            var fileName = $"{definition.Name}.{definition.Domain}.json";
            var fileFullPath = Path.Combine(folder.FullName, fileName);
            if (File.Exists(fileFullPath))
            {
                if (Repeat)
                {
                    var number = 1;
                    do
                    {
                        fileName = $"{definition.Name}.{definition.Domain}#{number}.json";
                        fileFullPath = Path.Combine(folder.FullName, fileName);
                        number++;
                    } while (File.Exists(fileFullPath));
                }
                else
                    return CallAnswer.LastCallAlreadyAnswered;
            }

            definition.WriteToFile(fileFullPath);
            return CallAnswer.CallAnswered;
        }

        public enum CallAnswer
        {
            NoCallToAnswerFound,
            LastCallAlreadyAnswered,
            CallAnswered
        }
    }
}
