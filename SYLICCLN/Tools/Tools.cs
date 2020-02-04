using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SYLICCLN.Tools
{
    public static class Logger
    {
        public static string LogFile { get; set; }

        public static void Entry(Exception e)
        {
            try
            {
                if (e == null)
                    return;

                string message = e.Message + "\r\n" + e.ToString();
                Entry(message);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        public static void Entry(string message, bool writeToLog = true, EventLogEntryType level = EventLogEntryType.Error)
        {
            try
            {
                if (!EventLog.SourceExists("PDI.Licenses"))
                {
                    EventLog.CreateEventSource("PDI.Licenses", "PDI.Licenses.Log");
                    EventLog.WriteEntry("Application", message, level, 55555);
                    return;
                }

                if (EventLog.SourceExists("PDI.Licenses"))
                    EventLog.WriteEntry("PDI.Licenses", message, level, 55555);
                else if (EventLog.SourceExists("Application"))
                    EventLog.WriteEntry("Application", message, level, 55555);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                try
                {
                    EventLog.WriteEntry("Application", message, level, 55555);
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
            }

            if (writeToLog)
            {
                try
                {
                    FileEntry(message);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        public static void FileEntry(string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(LogFile))
                    return;

                if (File.Exists(LogFile))
                {
                    FileInfo fi = new FileInfo(LogFile);
                    if (fi.Length >= 10000000)
                    {
                        string bakFile = LogFile + ".bak";
                        if (File.Exists(bakFile))
                            File.Delete(bakFile);
                        File.Move(LogFile, bakFile);
                    }
                }

                DateTime d = DateTime.Now;
                message = d.ToString("MM/dd/yyyy") + " " + d.ToString("hh:mm:ss") + ": " + message + "\r\n";
                File.AppendAllText(LogFile, message);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }
    }
}
