#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils.Helpers
{
    [ExcludeFromCoverage]
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