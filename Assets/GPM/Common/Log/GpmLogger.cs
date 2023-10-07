using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gpm.Common.Log
{
    public static class GpmLogger
    {
        public enum LogLevelType
        {
            NONE = 0,
            ERROR,
            WARN,
            DEBUG,
            ALL,
        }

        public static bool LogEnabled { get; set; } = true;

        public static LogLevelType LogLevel { get; set; } = LogLevelType.WARN;

        /// <summary>
        /// Generates a log message.
        /// </summary>
        /// <param name="message">required</param>
        /// <param name="serviceName">required</param>
        /// <param name="classType">required</param>
        /// <param name="methodName">optional</param>
        /// <returns>[GPM][ServiceName][ClassName::MethodName] message</returns>
        private static string MakeLog(object message, string serviceName, Type classType, string methodName)
        {
            StringBuilder log = new StringBuilder("[GPM]");
            log.AppendFormat("[{0}]", serviceName);
            log.AppendFormat("[{0}", classType.Name);
            log.AppendFormat("::{0}]", methodName);
            log.AppendFormat(" {0}", message);

            return log.ToString();
        }

        /// </summary>
        /// <summary>
        /// Debug log message.
        /// </summary>
        /// <param name="message">required</param>
        /// <param name="serviceName">required</param>
        /// <param name="classType">required</param>
        /// <param name="methodName">optional</param>
        /// <returns>[GPM][ServiceName][ClassName::MethodName] message</returns>
        public static void Debug(object message, string serviceName, Type classType,
#if CSHARP_7_3_OR_NEWER
            [CallerMemberName]
#endif
            string methodName = "")
        {
            if (GpmCommon.DebugLogEnabled == false)
            {
                if (LogEnabled == false)
                {
                    return;
                }

                if (LogLevel < LogLevelType.DEBUG)
                {
                    return;
                }
            }
            
            UnityEngine.Debug.Log(MakeLog(message, serviceName, classType, methodName));
        }

        /// </summary>
        /// <summary>
        /// Warning log message.
        /// </summary>
        /// <param name="message">required</param>
        /// <param name="serviceName">required</param>
        /// <param name="classType">required</param>
        /// <param name="methodName">optional</param>
        /// <returns>[GPM][ServiceName][ClassName::MethodName] message</returns>
        public static void Warn(object message, string serviceName, Type classType,
#if CSHARP_7_3_OR_NEWER
            [CallerMemberName] 
#endif
            string methodName = "")
        {
            if (GpmCommon.DebugLogEnabled == false)
            {
                if (LogEnabled == false)
                {
                    return;
                }

                if (LogLevel < LogLevelType.WARN)
                {
                    return;
                }
            }

            UnityEngine.Debug.LogWarning(MakeLog(message, serviceName, classType, methodName));
        }

        /// </summary>
        /// <summary>
        /// Error log message.
        /// </summary>
        /// <param name="message">required</param>
        /// <param name="serviceName">required</param>
        /// <param name="classType">required</param>
        /// <param name="methodName">optional</param>
        /// <returns>[GPM][ServiceName][ClassName::MethodName] message</returns>
        public static void Error(object message, string serviceName, Type classType,
#if CSHARP_7_3_OR_NEWER
            [CallerMemberName] 
#endif
            string methodName = "")
        {
            if (GpmCommon.DebugLogEnabled == false)
            {
                if (LogEnabled == false)
                {
                    return;
                }

                if (LogLevel < LogLevelType.ERROR)
                {
                    return;
                }
            }

            UnityEngine.Debug.LogError(MakeLog(message, serviceName, classType, methodName));
        }

        /// </summary>
        /// <summary>
        /// Exception log message.
        /// </summary>
        /// <param name="message">exception</param>
        public static void Exception(Exception exception)
        {
            if (GpmCommon.DebugLogEnabled == false)
            {
                if (LogEnabled == false)
                {
                    return;
                }

                if (LogLevel < LogLevelType.ERROR)
                {
                    return;
                }
            }

            UnityEngine.Debug.LogException(exception);
        }
    }
}
