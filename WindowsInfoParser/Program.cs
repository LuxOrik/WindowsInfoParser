using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WindowsInfoGatherer.Option;
using CommandLine;
using Newtonsoft.Json;

namespace WindowsInfoGatherer
{
    internal static class Program
    {
        /*
         * create /path/
         * answer /path/
         */
        private static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<AnswerOptions, CreateOptions>(args)
                                 .MapResult<AnswerOptions, CreateOptions, int>(DoAnswer,
                                     DoCreate,
                                     error => 1);
            }
            catch (DirectoryNotFoundException e)
            {
                LoggingUtil.Log(EventLogEntryType.Error, 1500, e.Message);
                return 2;
            }
        }

        private static int DoCreate(CreateOptions create)
        {
            if (create.ScheduledDate == null
                || !DateTime.TryParse(create.ScheduledDate, out var date))
                date = DateTime.Today;
            
            switch (CallFolderManager.CreateNewDefinitionCall(create.FolderPath, date))
            {
                case CreateResult.AlreadyExists:
                    LoggingUtil.Log(EventLogEntryType.Error,
                        1510,
                        create.ScheduledDate == null
                            ? "A call already exists for today."
                            : $"A call already exists for that date ({date.ToString(CallFolderManager.Format)}).");
                    return 3;
                case CreateResult.Ok:
                    LoggingUtil.Log(EventLogEntryType.Information, 1511, "Call created.");
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int DoAnswer(AnswerOptions answer)
        {
            switch (answer.AnswerCall())
            {
                case AnswerOptions.CallAnswer.NoCallToAnswerFound:
                    LoggingUtil.Log(EventLogEntryType.Warning, 1520, "No call found.");
                    return 0;
                case AnswerOptions.CallAnswer.LastCallAlreadyAnswered:
                    LoggingUtil.Log(EventLogEntryType.Information, 1521, "No new call to answer.");
                    return 0;
                case AnswerOptions.CallAnswer.CallAnswered:
                    LoggingUtil.Log(EventLogEntryType.Information, 1522, "New call answered.");
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
