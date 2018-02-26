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
         * create /path/ [/date/]
         * answer /path/
         * export /path/ /fileout/ /format/
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
                LoggingUtil.Log(EventLogEntryType.Error, EventLogIds.DirectoryNotFound.Id(), e.Message);
                return 2;
            }
        }

        private static int DoCreate(CreateOptions create)
        {
            switch (create.Execute())
            {
                case CreateResult.AlreadyExists:
                    LoggingUtil.Log(
                        EventLogEntryType.Error,
                        EventLogIds.CallAlreadyExists.Id(),
                        create.ScheduledDate == null
                            ? "A call already exists for today."
                            : $"A call already exists for that date ({create.UsedDate?.ToString(CallFolderManager.Format) ?? "Undefined date"}).");
                    return 3;
                case CreateResult.Ok:
                    LoggingUtil.Log(EventLogEntryType.Information, EventLogIds.CallCreated.Id(), "Call created.");
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int DoAnswer(AnswerOptions answer)
        {
            switch (answer.Execute())
            {
                case AnswerOptions.CallAnswer.NoCallToAnswerFound:
                    LoggingUtil.Log(EventLogEntryType.Warning, EventLogIds.NoCallAtAll.Id(), "No call found.");
                    return 0;
                case AnswerOptions.CallAnswer.LastCallAlreadyAnswered:
                    LoggingUtil.Log(EventLogEntryType.Information, EventLogIds.CallAlreadyAnswered.Id(), "No new call to answer.");
                    return 0;
                case AnswerOptions.CallAnswer.CallAnswered:
                    LoggingUtil.Log(EventLogEntryType.Information, EventLogIds.CallAnswered.Id(), "New call answered.");
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int DoExport(ExportOptions export)
        {
            export.Execute();
            LoggingUtil.Log(EventLogEntryType.Information, EventLogIds.DoNotEventLog.Id(), "Done.");
            return 0;
        }

        private static int Id(this EventLogIds id) => (int) id;

        internal enum EventLogIds
        {
            DoNotEventLog = -1,
            DirectoryNotFound = 1500,
            CallAlreadyExists = 1510,
            CallCreated = 1511,
            NoCallAtAll = 1520,
            CallAlreadyAnswered = 1521,
            CallAnswered = 1522

        }
    }
}
