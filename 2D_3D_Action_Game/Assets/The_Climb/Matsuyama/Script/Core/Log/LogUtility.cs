using UnityEngine;

namespace TheClimb.Logging
{
    public static class LogUtility
    {
        public static LogLevel CurrentLogLevel =
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        LogLevel.Debug;
#else
        LogLevel.Warning;
#endif

        public static void Log(LogPrefix logPrefix, string message, LogLevel level)
        {
            if (level < CurrentLogLevel) { return; }

            if (level == LogLevel.Error)
            {
                Debug.LogError($"[LogLevel: {level}] {logPrefix} {message}");
            }
            else if (level == LogLevel.Warning)
            {
                Debug.LogWarning($"[LogLevel: {level}] {logPrefix} {message}");
            }
            else
            {
                Debug.Log($"[LogLevel: {level}] {logPrefix} {message}");
            }
        }
    }
}