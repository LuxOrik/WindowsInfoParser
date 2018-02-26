using System;
using System.Diagnostics;
using System.Security;

namespace WindowsInfoGatherer
{
    internal static class LoggingUtil
    {
        private const string CategoryLog = "Application";
        private const string SourceLog = nameof(WindowsInfoGatherer);
        private static readonly bool EventLogReachable = true;

        static LoggingUtil()
        {
            try
            {
                if (!EventLog.SourceExists(SourceLog))
                    EventLog.CreateEventSource(new EventSourceCreationData(SourceLog, CategoryLog));
            }
            catch (SecurityException)
            {
                EventLogReachable = false;
            }
        }

        public static void Log(EventLogEntryType type, int eventId, string message)
        {
            if (EventLogReachable)
                EventLog.WriteEntry(SourceLog, message, type, eventId);

            (type == EventLogEntryType.Error ? Console.Error : Console.Out).WriteLine($"{eventId:D4}: {message}");
        }
    }
}
