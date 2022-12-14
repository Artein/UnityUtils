using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUtils.Extensions;

namespace Extensions
{
    [TestFixture] public class PlayerPrefsExtTests
    {
        private const string TestsSaveKey = "TEST_SAVE_KEY";

        [SetUp] public void SetUp()
        {
            PlayerPrefs.DeleteKey(TestsSaveKey);
        }

        [TearDown] public void TearDown()
        {
            PlayerPrefs.DeleteKey(TestsSaveKey);
        }
        
        [Test] public void TryGetBool_ReturnsFalse_WhenNoKeyWasSaved()
        {
            var receivedBool = PlayerPrefsExt.TryGetBool(TestsSaveKey, out _);

            receivedBool.Should().Be(false);
        }

        [Test] public void TryGetBool_SetOutArgument_Value_ToNull_WhenNoKeyWasSaved()
        {
            PlayerPrefsExt.TryGetBool(TestsSaveKey, out var outValue);

            outValue.HasValue.Should().Be(false);
        }

        [Test] public void TryGetBool_ReturnsFalse_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetBool works on Strings
            
            var receivedBool = PlayerPrefsExt.TryGetBool(TestsSaveKey, out _);
            
            receivedBool.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetBool)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetBool_SetOutArgument_Value_ToNull_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetBool works on Strings
            
            PlayerPrefsExt.TryGetBool(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetBool)}: Could not parse loaded string ()");
        }

        [Test] public void TryGetBool_ReturnsFalse_WhenSavedKey_CantBeParsedToBoolean()
        {
            const string customString = "DEFINITELY_NOT_A_BOOLEAN";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetBool works on Strings
            
            var receivedBool = PlayerPrefsExt.TryGetBool(TestsSaveKey, out _);
            
            receivedBool.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetBool)}: Could not parse loaded string ({customString})");
        }
        
        [Test] public void TryGetBool_SetOutArgument_Value_ToNull_WhenSavedKey_CantBeParsedToBoolean()
        {
            const string customString = "DEFINITELY_NOT_A_BOOLEAN";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetBool works on Strings
            
            PlayerPrefsExt.TryGetBool(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetBool)}: Could not parse loaded string ({customString})");
        }
    }
}