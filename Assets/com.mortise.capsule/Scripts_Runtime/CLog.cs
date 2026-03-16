using System;

namespace MortiseFrame.Capsule {

    public static class CLog {
        public static Action<string> LogHandler;
        internal static void Log(string message) {
            LogHandler?.Invoke(message);
        }

        public static Action<string> LogWarningHandler;
        internal static void LogWarning(string message) {
            LogWarningHandler?.Invoke(message);
        }

        public static Action<string> LogErrorHandler;
        internal static void LogError(string message) {
            LogErrorHandler?.Invoke(message);
        }
    }

}