using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WindowsInfoParser.Option;
using CommandLine;
using Newtonsoft.Json;

namespace WindowsInfoParser
{
    internal static class Program
    {
        /*
         * create /path/
         * answer /path/
         */
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<AnswerOptions, CreateOptions>(args)
                         .MapResult<AnswerOptions, CreateOptions, int>(DoAnswer,
                             DoCreate,
                             error => 1);
        }

        private static int DoCreate(CreateOptions create)
        {
            if (create.ScheduledDate == null
                || !DateTime.TryParse(create.ScheduledDate, out var date))
                date = DateTime.Today;
            
            switch (CallFolderManager.CreateNewDefinitionCall(create.FolderPath, date))
            {
                case CreateResult.AlreadyExists:
                    Console.Error.WriteLine(create.ScheduledDate == null
                        ? "A call already exists for today"
                        : $"A call already exists for that date ({date.ToString(CallFolderManager.Format)})");
                    return 2;
                case CreateResult.Ok:
                    Console.WriteLine("Call created");
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
                    Console.WriteLine("No call to answer");
                    return 0;
                case AnswerOptions.CallAnswer.LastCallAlreadyAnswered:
                    Console.WriteLine("Last call was already answered");
                    return 0;
                case AnswerOptions.CallAnswer.CallAnswered:
                    Console.WriteLine("New call answered");
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
