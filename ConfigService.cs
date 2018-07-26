using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;

namespace OsrsColorBot
{
    public static class ConfigService
    {
        public static UserConfigOptionsModel LoadConfigData()
        {
            var response = new UserConfigOptionsModel();

            var configFilename = ConfigurationManager.AppSettings["UserConfigFileName"];
            var currentDir = Directory.GetCurrentDirectory();

            var di = new DirectoryInfo(currentDir).GetFiles().Where(x => x.Name == configFilename).ToList();

            if (!di.Any())
            {
                response = GetDefaultUserOptions(currentDir);

                using (var fileStream = new FileStream(String.Format("{0}\\{1}", currentDir, configFilename), FileMode.Create))
                using (var fileWriter = new StreamWriter(fileStream))
                {
                    fileWriter.Write(response.ToXML());
                }
            }
            else
            {
                using (var fileStream = new FileStream(String.Format("{0}\\{1}", currentDir, configFilename), FileMode.Open))
                using (var fileReader = new StreamReader(fileStream))
                {
                    var xmlData = fileReader.ReadToEnd();

                    response = UserConfigOptionsModel.LoadFromXMLString(xmlData);
                }
            }

            return response;
        }

        public static void SaveUserConfigOptions(UserConfigOptionsModel userConfigOptions)
        {
            var configFilename = ConfigurationManager.AppSettings["UserConfigFileName"];

            using (var fileStream = new FileStream(String.Format("{0}\\{1}", LoggingUtility.CurrentWorkingDirectory, configFilename), FileMode.Create))
            using (var fileWriter = new StreamWriter(fileStream))
            {
                fileWriter.Write(userConfigOptions.ToXML());
            }
        }

        public static void CheckResourceDirectoryExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private static UserConfigOptionsModel GetDefaultUserOptions(string currentWorkingDirectory)
        {
            var response = new UserConfigOptionsModel();

            response.ResourceLocation = String.Format("{0}\\{1}", currentWorkingDirectory, ConfigurationManager.AppSettings["DefaultBaseResourceLocation"]);
            response.TestingFlag = ConfigurationManager.AppSettings["DefaultTestingFlag"];

            return response;
        }
    }
}
