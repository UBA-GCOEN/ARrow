using System;
using System.Diagnostics;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Logging class that collects various logging facilities under one static class.
    /// </summary>
    public static class MagicLeapLogger
    {
        /// <summary>
        /// The logging level to use. Ranges from 'Verbose' to 'Fatal'
        /// </summary>
        public enum LogLevel : uint {
            /// <summary>
            /// Output a fatal error which causes program termination.
            /// </summary>
            Fatal = 0,
            /// <summary>
            /// Output a serious error. The program may continue.
            /// </summary>
            Error = 1,
            /// <summary>
            /// Output a warning which may be ignorable.
            /// </summary>
            Warning = 2,
            /// <summary>
            /// Output an informational message.
            /// </summary>
            Info = 3,
            /// <summary>
            /// Output a message used during debugging.
            /// </summary>
            Debug = 4,
            /// <summary>
            /// Output a message used for noisier informational messages.
            /// </summary>
            Verbose = 5
        }
        static class Native
        {
            public static void Log(LogLevel level, string tag, string message)
            {
                switch (level)
                {
                    case LogLevel.Fatal:
                    case LogLevel.Error:
                        UnityEngine.Debug.LogErrorFormat("[{0}]: {1}", tag, message);
                        break;
                    case LogLevel.Warning:
                        UnityEngine.Debug.LogWarningFormat("[{0}]: {1}", tag, message);
                        break;
                    case LogLevel.Info:
                    case LogLevel.Debug:
                    case LogLevel.Verbose:
                    default:
                        UnityEngine.Debug.LogFormat("[{0}]: {1}", tag, message);
                        break;
                }
            }
        }

        /// <summary>
        /// Assert
        /// </summary>
        /// <param name="condition">Condition to test against</param>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String formatting</param>
        /// <param name="args">Arguments to pass into `format`</param>
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, string tag, string format, params object[] args)
        {
            Assert(condition, LogLevel.Debug, tag, format, args);
        }

        /// <summary>
        /// Assert
        /// </summary>
        /// <param name="condition">Condition to test against</param>
        /// <param name="level">Logging level</param>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String formatting</param>
        /// <param name="args">Arguments to pass into `format`</param>
        internal static void Assert(bool condition, LogLevel level, string tag, string format, params object[] args)
        {
            if (!condition)
                Native.Log(level, tag, string.Format(format, args));
        }

        /// <summary>
        /// Assert and Error
        /// </summary>
        /// <param name="condition">Condition to test against</param>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String formatting</param>
        /// <param name="args">Arguments to pass into `format`</param>
        public static void AssertError(bool condition, string tag, string format, params object[] args)
        {
            Assert(condition, LogLevel.Error, tag, format, args);
        }

        /// <summary>
        /// Log a Debug message
        /// </summary>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String Format for the message</param>
        /// <param name="args">Arguments to pass into `format`</param>
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Debug(string tag, string format, params object[] args)
        {
            Native.Log(LogLevel.Debug, tag, string.Format(format, args));
        }

        /// <summary>
        /// Log a Warning message
        /// </summary>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String Format for the message</param>
        /// <param name="args">Arguments to pass into `format`</param>
        public static void Warning(string tag, string format, params object[] args)
        {
            Native.Log(LogLevel.Warning, tag, string.Format(format, args));
        }

        /// <summary>
        /// Log an Error message
        /// </summary>
        /// <param name="tag">Reference Tag</param>
        /// <param name="format">String Format for the message</param>
        /// <param name="args">Arguments to pass into `format`</param>
       public static void Error(string tag, string format, params object[] args)
        {
            Native.Log(LogLevel.Error, tag, string.Format(format, args));
        }
    }
}