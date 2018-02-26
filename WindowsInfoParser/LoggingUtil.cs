using System;
using System.Diagnostics;
using System.Security;

namespace WindowsInfoGatherer
{
    internal static class LoggingUtil
    {
        private const string CategoryLog = "Application";
        private const string SourceLog = nameof(WindowsInfoGatherer);

        static LoggingUtil()
        {
            try
            {
                if (!EventLog.SourceExists(SourceLog))
                    EventLog.CreateEventSource(new EventSourceCreationData(SourceLog, CategoryLog));
            }
            catch (SecurityException)
            {
                
            }
        }

        public static void Log(EventLogEntryType type, int eventId, string message)
        {
            if (eventId > 0)
            {
                try
                {
                    EventLog.WriteEntry(SourceLog, message, type, eventId);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Could not write the event. The source must not exists. Launch this program once with admin rights to create the source.");
                }
            }

            (type == EventLogEntryType.Error ? Console.Error : Console.Out).WriteLine($"{eventId:D4}: {message}");
        }
    }
}
