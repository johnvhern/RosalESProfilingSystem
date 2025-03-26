using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosalESProfilingSystem
{
    internal class ActivityLogger
    {
        private string logFilePath;

        public ActivityLogger()
        {
            // Get the AppData directory for the current user
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RosalES");

            // Ensure the directory exists
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            logFilePath = Path.Combine(appDataFolder, "ActivityLogs.txt");
        }

        public void LogActivity(string username, string action)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {username} | {action}";

                using (StreamWriter sw = new StreamWriter(logFilePath, true)) // Append mode
                {
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to log file: " + ex.Message);
            }
        }

        public List<string> ReadLogs()
        {
            List<string> logs = new List<string>();

            try
            {
                if (File.Exists(logFilePath))
                {
                    logs = new List<string>(File.ReadAllLines(logFilePath));
                }
                else
                {
                    logs.Add("No logs found.");
                }
            }
            catch (Exception ex)
            {
                logs.Add("Error reading log file: " + ex.Message);
            }

            return logs;
        }
    }
}
