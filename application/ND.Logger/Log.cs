using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace Logger
{
    public static class Log
    {
        // enum with only available log types
        public enum Type
        {
            Fatal, Error, Info, Warn, Debug, All
        }

        // for testing saved settings in ".config" file
        public static string ShowSettings()
        {
            string SettingString = "";
            const string endl = "\r\n";
            GeneralConfig config = GeneralConfig.Config;
            SettingString += string.Format("Logs root dir: {0}", config.RootDir) + endl;
            for (int i = 0; i < config.Settings.Count; i++)
            {
                LogElement logConfig = config.Settings[i];
                SettingString += string.Format("Log name: {0}, dir: {1}, file: {2}, max log size (MB): {3}, save: {4}", logConfig.Name, logConfig.Dir, logConfig.File, logConfig.MaxLogSize, logConfig.Save) + endl;
            }

            return SettingString;
        }

        private static void WriteLog(string msg, LogElement logConfig, bool verbose = false)
        {
            //
            // <log name="Fatal" dir="Fatal" file="log.txt" maxLogSize="20" save="true" ></log>

            // Creates log folder if it does not exist
            Folder logFolder = new Folder(logConfig.Dir);
            string time = DateTime.Now.ToLocalTime().ToString("u");
            string record = string.Format("[{0}]\t{1} ", time, msg);
            string logPath = Path.Combine(logFolder.LogPath, logConfig.File);
            LogFile logFile = new LogFile(logPath, logConfig);
            
            // If file is too big archive it & rename it
            if(logFile.TooBig())
            {                
                logFile.Archive();
                logFile.Remove();
            }           
            
            using (StreamWriter sw = new StreamWriter(logPath, true))
            {
                sw.WriteLine(record);
                sw.Flush();
            }
            // To check correct file writing
            if (verbose)
            {             
                Console.WriteLine(">> {2}/{0} = {1}", logFile.Info.Name, logFile.SizeMB.ToString(), logFile.Info.Directory.Name);
            }

        }
        private static void ProcessLog(string msg, string methodName, bool verbose)
        {
            LogElement logConfig = ConfigFromMethodName(methodName);
            if (logConfig.Save)
            {
                // Write to appropriate log
                WriteLog(msg, logConfig, verbose);

                // Every message is written to All/log.txt too
                LogElement allConfig = GeneralConfig.Config.Settings[Log.Type.All];
                if (allConfig.Save)
                {
                    string allMsg = string.Format("[{0}]\t{1}", logConfig.Name.ToString(), msg);
                    WriteLog(allMsg, allConfig, verbose);
                }
            }
        }

        //log types are Fatal, Error, Info, Warn, Debug, All (see Log.LogType)
   
        internal static double BytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        // Gets log config according to parsed method name to Log.LogType
        private static LogElement ConfigFromMethodName(string methodName)
        {
            Log.Type logType = (Log.Type)Enum.Parse(typeof(Log.Type), methodName);
            return GeneralConfig.Config.Settings[logType];
        }

        #region LoggingMethods
        // Method of "Fatal error" Logs
        public static void Fatal(string msg, bool verbose = false)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToString();
            ProcessLog(msg, methodName, verbose);
        }

        // Method of "Error" Logs
        public static void Error(string msg, bool verbose = false)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToString();
            ProcessLog(msg, methodName, verbose);
        }

        // Method of "Info" Logs
        public static void Info(string msg, bool verbose = false)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToString();
            ProcessLog(msg, methodName, verbose);
        }

        // Method of "Warning" Logs
        public static void Warn(string msg, bool verbose = false)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToString();
            ProcessLog(msg, methodName, verbose);
        }

        // Method of "Debug" Logs
        public static void Debug(string msg, bool verbose = false)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToString();
            ProcessLog(msg, methodName, verbose);
        }
        #endregion
    }
}
