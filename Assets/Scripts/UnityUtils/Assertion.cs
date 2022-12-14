using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils
{
    public static class Assertion
    {
        public static void Expect_LogException_NotImplementedException()
        {
            LogAssert.Expect(LogType.Exception, "NotImplementedException: The method or operation is not implemented.");
        }
    }
}