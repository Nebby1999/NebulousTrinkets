using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NebulousTrinkets
{
    public class NLog
    {
        public static ManualLogSource logger = null;

        public NLog(ManualLogSource logger_)
        {
            logger = logger_;
        }

        public static void Debug(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogDebug(LogString(data, i, member));
        }
        public static void Error(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogError(LogString(data, i, member));
        }
        public static void Fatal(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogFatal(LogString(data, i, member));
        }
        public static void Info(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogInfo(LogString(data, i, member));
        }
        public static void Message(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogMessage(LogString(data, i, member));
        }
        public static void Warning(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogWarning(LogString(data, i, member));
        }

        private static string LogString(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            return string.Format("{0} :: Line: {1}, Method {2}", data, i, member);
        }
    }
}