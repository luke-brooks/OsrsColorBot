using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;

namespace OsrsColorBot
{
    public static class LoggingUtility
    {
        public static string CurrentWorkingDirectory
        {
            get { return Directory.GetCurrentDirectory(); }
        }

        public static void WriteToAuditLog(string auditMessage)
        {
            WriteToLog(auditMessage, ConfigurationManager.AppSettings["AuditLogName"]);
        }

        public static void WriteToErrorLog(Exception ex)
        {
            WriteToLog(WriteAllExceptionMessages(ex), ConfigurationManager.AppSettings["ErrorLogName"]);
        }

        public static void WriteToColorLog(List<OsrsColor> colorList, string imageName)
        {
            using (var fileStream = new FileStream(String.Format("{0}\\{1}.txt", CurrentWorkingDirectory, imageName), FileMode.Create))
            using (var fileWriter = new StreamWriter(fileStream))
            {
                fileWriter.WriteLine(String.Format("{0}", ToXML(colorList)));
            }
        }

        public static void SaveScreenshot(Bitmap screenshot)
        {
            using (var fileStream = new FileStream(String.Format("{0}\\{1}.bmp", CurrentWorkingDirectory, "screenshot"), FileMode.Create))
            using (var fileWriter = new StreamWriter(fileStream))
            {
                screenshot.Save(fileStream, ImageFormat.Bmp);
            }
        }

        private static string ToXML(List<OsrsColor> colorList)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(colorList.GetType());
            serializer.Serialize(stringwriter, colorList);
            return stringwriter.ToString();
        }

        private static List<OsrsColor> LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(List<OsrsColor>));
            return serializer.Deserialize(stringReader) as List<OsrsColor>;
        }

        private static void WriteToLog(string logMessage, string logName)
        {
            var di = new DirectoryInfo(CurrentWorkingDirectory).GetFiles().Where(x => x.Name == logName).ToList();

            var fm = FileMode.Append;
            if (!di.Any())
            {
                fm = FileMode.Create;
            }

            using (var fileStream = new FileStream(String.Format("{0}\\{1}", CurrentWorkingDirectory, logName), fm))
            using (var fileWriter = new StreamWriter(fileStream))
            {
                fileWriter.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToString(), logMessage));
            }
        }

        private static void WriteColor(string colorData, string logName)
        {
        }

        private static string WriteAllExceptionMessages(Exception ex)
        {
            StringBuilder msgBuilder = new StringBuilder();

            msgBuilder.Append(String.Format("Exception Message : {0}  ", ex.Message));

            if (ex.InnerException != null)
            {
                msgBuilder.Append(WriteAllExceptionMessages(ex.InnerException));
            }

            return msgBuilder.ToString();
        }
    }
}
