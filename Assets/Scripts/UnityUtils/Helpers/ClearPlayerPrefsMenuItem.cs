#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils.Helpers
{
    public static class ClearPlayerPrefsMenuItem
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        public static void Invoke()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}

#endif // UNITY_EDITOR