using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsInfoParser
{
    public static class CallAnswerer
    {
        public static CallAnswer AnswerCall()
        {
            var today = DateTime.Today;
            var lowerLimit = today - TimeSpan.FromDays(365 * 2);
            var (folder, date) = DefinitionCallFolderManager.GetAllDefinitionCalls().FirstOrDefault(call => call.Date <= today);
            if (date < lowerLimit)
                return CallAnswer.NoCallToAnswerFound;

            var definition = SimplePC.GetCurrentMachineDefinition();
            var fileName = $"{definition.Name}.json";
            var fileFullPath = Path.Combine(folder.FullName, fileName);
            if (File.Exists(fileFullPath))
                return CallAnswer.LastCallAlreadyAnswered;

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
