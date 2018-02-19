using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsInfoParser
{
    internal static class Program
    {
        /*
         * -CreateNewCall OR -AnswerCall
         * [-SetWorkingDirectory]
         */
        private static void Main(string[] args)
        {
            CallAnswerer.AnswerCall();
        }
    }
}
