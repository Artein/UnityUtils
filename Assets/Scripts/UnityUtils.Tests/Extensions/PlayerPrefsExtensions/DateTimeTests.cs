using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUtils.Extensions;

namespace Extensions.PlayerPrefsExtensions
{
    [TestFixture] public class DateTimeTests
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
        
        [Test] public void SetDateTime_SavesIntoString_UsingTicks()
        {
            DateTime value = new DateTime(999999);
            PlayerPrefsExt.SetDateTime(TestsSaveKey, value);

            var savedString = PlayerPrefs.GetString(TestsSaveKey);

            savedString.Should().Be(value.Ticks.ToString());
        }
        
        [Test] public void TryGetDateTime_ReturnsTrue_WhenSuccessfullyLoadedFromSave()
        {
            DateTime saveValue = new DateTime(999999);
            PlayerPrefsExt.SetDateTime(TestsSaveKey, saveValue);
            
            var success = PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out _);

            success.Should().Be(true);
        }
        
        [Test] public void TryGetDateTime_SetOutArgument_Value_WhenSuccessfullyLoadedFromSave()
        {
            DateTime saveValue = new DateTime(999999);
            PlayerPrefsExt.SetDateTime(TestsSaveKey, saveValue);
            
            PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out var outValue);
            
            outValue!.Value.Should().Be(saveValue);
        }

        [Test] public void TryGetDateTime_ReturnsFalse_WhenNoKeyWasSaved()
        {
            var success = PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out _);

            success.Should().Be(false);
        }
        
        [Test] public void TryGetDateTime_SetOutArgument_Value_ToNull_WhenNoKeyWasSaved()
        {
            PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out var outValue);

            outValue.HasValue.Should().Be(false);
        }
        
        [Test] public void TryGetDateTime_ReturnsFalse_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetDateTime works on Strings
            
            var success = PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetDateTime)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetDateTime_SetOutArgument_Value_ToNull_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetDateTime works on Strings
            
            PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetDateTime)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetDateTime_ReturnsFalse_WhenSavedKey_CantBeParsedToDateTime()
        {
            const string customString = "DEFINITELY_NOT_A_DATETIME";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetDateTime works on Strings
            
            var success = PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetDateTime)}: Could not parse loaded string ({customString})");
        }
        
        [Test] public void TryGetDateTime_SetOutArgument_Value_ToNull_WhenSavedKey_CantBeParsedToDateTime()
        {
            const string customString = "DEFINITELY_NOT_A_DATETIME";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetDateTime works on Strings
            
            PlayerPrefsExt.TryGetDateTime(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetDateTime)}: Could not parse loaded string ({customString})");
        }
    }
}