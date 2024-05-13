using System;
using UnityEngine;

namespace Monaverse
{
    public static class MonaDebug
    {
        public static bool IsEnabled { get; set; } = true;
        private const string Prefix = "[Monaverse] ";

        public static void Log(object message)
        {
            if (IsEnabled)
                Debug.Log(Prefix + message);
        }

        public static void LogWarning(object message)
        {
            if (IsEnabled)
                Debug.LogWarning(Prefix + message);
        }

        public static void LogError(object message)
        {
            if (IsEnabled)
                Debug.LogError(Prefix + message);
        }

        public static void LogException(Exception exception)
        {
            if (IsEnabled)
                Debug.LogException(exception);
        }
    }
}