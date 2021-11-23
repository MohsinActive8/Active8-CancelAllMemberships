using System;
using System.IO;

namespace Active8_CancelAllMemberships.Utility
{
    public enum LogStatus
    {
        info,
        success,
        error,
    };
    public class Logger
    {
        
        public static void Log(string message)
        {
            message = "[" + DateTime.Now.ToString("G") + "] >> " + message;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();

            // writing error into file
            using (StreamWriter sw = File.AppendText(AppContext.BaseDirectory + "InfoLog.txt"))
            {
                sw.WriteLine(message);
                sw.Flush();
            }
        }
        public static void CustomLog(string message, LogStatus status)
        {
            message = "[" + DateTime.Now.ToString("G") + "] >> " + message;
            if(status == LogStatus.info)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }else if(status == LogStatus.success) 
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }else if (status == LogStatus.error) 
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }

            Console.WriteLine(message);
            Console.ResetColor();

            // writing error into file
            using (StreamWriter sw = File.AppendText(AppContext.BaseDirectory + "InfoLog.txt"))
            {
                sw.WriteLine(message);
                sw.Flush();
            }
        }

        public static void LogError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[" + DateTime.Now.ToString("G") + "] >> " + ex.Message);
            Console.WriteLine("---------Stack Trace---------");
            Console.WriteLine("[" + DateTime.Now.ToString("G") + "] >> " + ex.StackTrace);
            Console.ResetColor();

            // writing error into file
            using (StreamWriter sw = File.AppendText(AppContext.BaseDirectory + "ErrorLog.txt"))
            {
                sw.WriteLine("****************************************** ["+DateTime.Now.ToString("G") + "] ******************************************\n");
                sw.WriteLine(ex.Message+"\n");
                sw.WriteLine("------- Stack Trace --------");
                sw.WriteLine(ex.StackTrace+"\n");
                sw.WriteLine("****************************************** [END] ******************************************\n");
                sw.Flush();
            }
        }

        public static void LogSql(string query)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[" + DateTime.Now.ToString("G") + "] >> " + query);
            Console.ResetColor();
        }
    }
}
