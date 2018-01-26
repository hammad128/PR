using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureService
{
    public static class Logger
    {
        public static void WriteLog(string Module, string Message, string Details = "", string DBObjectPK = "")
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + @"\";
                if (!Directory.Exists(path + "Log-Flush"))
                {
                    Directory.CreateDirectory(path + "Log-Flush");
                }
                if (!Directory.Exists(path + @"Log-Flush\" + DateTime.Now.ToString("MMMM")))
                {
                    Directory.CreateDirectory(path + @"Log-Flush\" + DateTime.Now.ToString("MMMM"));
                }
                StreamWriter sw = new StreamWriter(path + @"Log-Flush\" + DateTime.Now.ToString("MMMM") + @"\ServiceWorking-" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", true);

                string msg = "";
                if (!string.IsNullOrEmpty(Module))
                {
                    msg = AppendString(msg, "Module: " + Module, ",");
                }
                if (!string.IsNullOrEmpty(DBObjectPK))
                {
                    msg = AppendString(msg, "Key: " + DBObjectPK, ",");
                }
                if (!string.IsNullOrEmpty(Message))
                {
                    msg = AppendString(msg, "Message: " + Message, ",");
                }
                if (!string.IsNullOrEmpty(Details))
                {
                    msg = AppendString(msg, " Details: " + Details, ",");
                }
                msg = AppendString(msg, "--> at " + DateTime.Now.ToString(), "");

                sw.WriteLine(msg);
                sw.Close();
            }
            catch (Exception ex)
            {
                // eventLog1.WriteEntry("ManageCache - Ex :" + ex.InnerException.Message, EventLogEntryType.Error);
            }
        }

        private static string AppendString(string mainString, string toAppend, string endingChar)
        {
            if (!string.IsNullOrWhiteSpace(mainString) && !string.IsNullOrWhiteSpace(toAppend))
            {
                mainString += endingChar + " ";
            }
            mainString += toAppend;
            return mainString;
        }
    }
}
