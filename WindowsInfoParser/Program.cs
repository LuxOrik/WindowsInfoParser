using System;
using System.Diagnostics;
using System.IO;
using WindowsInfoGatherer.Option;
using CommandLine;

namespace WindowsInfoGatherer
{
    internal static class Program
    {
#if DEBUG
        public const bool Debug = true;
#else
        public const bool Debug = false;
#endif

        /*
         * create /path/
         * answer /path/
         */
        private static int Main(string[] args)
        {
            try
            {
                var retCode = Parser.Default.ParseArguments<AnswerOptions, CreateOptions, ExportOptions>(args)
                                    .MapResult<AnswerOptions, CreateOptions, ExportOptions, int>(
                                        DoAnswer,
                                        DoCreate,
                                        DoExport,
                                        error => 1);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (Debugger.IsAttached && Debug)
                {
                    Console.WriteLine("Press any key...");
                    Console.ReadKey();
                }
                return retCode;
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

        private static int DoExport(ExportOptions export)
        {
            export.Execute();
            Console.WriteLine("Done."); //Do not write to eventlog
            return 0;
        }
    }
}
