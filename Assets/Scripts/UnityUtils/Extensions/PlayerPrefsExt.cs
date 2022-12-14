using System;
using UnityEngine;

namespace UnityUtils.Extensions
{
    public static class PlayerPrefsExt
    {
        public static bool TryGetBool(string key, out bool? value)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                value = null;
                return false;
            }

            var valueStr = PlayerPrefs.GetString(key);
            if (!bool.TryParse(valueStr, out var parsedValue))
            {
                Debug.LogError($"{nameof(PlayerPrefsExt.TryGetBool)}: Could not parse loaded string ({valueStr})");
                value = null;
                return false;
            }

            value = parsedValue;
            return true;
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static bool TryGetTimeSpan(string key, out TimeSpan? value)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                value = null;
                return false;
            }

            var timeSpanStr = PlayerPrefs.GetString(key);
            if (!TimeSpan.TryParse(timeSpanStr, out var parsedTimeSpan))
            {
                Debug.LogError($"{nameof(PlayerPrefsExt.TryGetTimeSpan)}: Could not parse loaded string ({timeSpanStr})");
                value = null;
                return false;
            }

            value = parsedTimeSpan;
            return true;
        }

        public static void SetTimeSpan(string key, TimeSpan value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static bool TryGetDateTime(string key, out DateTime? value)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                value = null;
                return false;
            }

            var ticksStr = PlayerPrefs.GetString(key);
            if (!long.TryParse(ticksStr, out var ticks))
            {
                Debug.LogError($"{nameof(PlayerPrefsExt.TryGetDateTime)}: Could not parse loaded string ({ticksStr})");
                value = null;
                return false;
            }

            value = new DateTime(ticks);
            return true;
        }

        public static void SetDateTime(string key, DateTime value)
        {
            PlayerPrefs.SetString(key, value.Ticks.ToString());
        }

        public static bool TryGetGuid(string key, out Guid? value)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                value = null;
                return false;
            }

            var guidStr = PlayerPrefs.GetString(key);
            if (!Guid.TryParseExact(guidStr, GuidFormat, out var parsedGuid))
            {
                Debug.LogError($"{nameof(PlayerPrefsExt.TryGetGuid)}: Could not parse loaded string ({guidStr})");
                value = null;
                return false;
            }

            value = parsedGuid;
            return true;
        }

        public static void SetGuid(string key, Guid value)
        {
            PlayerPrefs.SetString(key, value.ToString(GuidFormat));
        }

        private const string GuidFormat = "N"; // using Number format as the shortest one
    }
}