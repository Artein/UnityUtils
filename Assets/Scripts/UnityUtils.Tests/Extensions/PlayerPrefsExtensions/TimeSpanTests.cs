using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUtils.Extensions;

namespace Extensions.PlayerPrefsExtensions
{
    [TestFixture] public class TimeSpanTests
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
        
        [Test] public void SetTimeSpan_SavesIntoString()
        {
            TimeSpan value = new TimeSpan(12345);
            PlayerPrefsExt.SetTimeSpan(TestsSaveKey, value);

            var savedString = PlayerPrefs.GetString(TestsSaveKey);

            savedString.Should().Be(value.ToString());
        }
        
        [Test] public void TryGetTimeSpan_ReturnsTrue_WhenSuccessfullyLoadedFromSave()
        {
            PlayerPrefsExt.SetTimeSpan(TestsSaveKey, new TimeSpan(12345));
            
            var success = PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out _);

            success.Should().Be(true);
        }
        
        [Test] public void TryGetTimeSpan_SetOutArgument_Value_WhenSuccessfullyLoadedFromSave()
        {
            TimeSpan saveValue = new TimeSpan(12345);
            PlayerPrefsExt.SetTimeSpan(TestsSaveKey, saveValue);
            
            PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out var outValue);
            
            outValue!.Value.Should().Be(saveValue);
        }
        
        [Test] public void TryGetTimeSpan_ReturnsFalse_WhenNoKeyWasSaved()
        {
            var success = PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out _);

            success.Should().Be(false);
        }

        [Test] public void TryGetTimeSpan_SetOutArgument_Value_ToNull_WhenNoKeyWasSaved()
        {
            PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out var outValue);

            outValue.HasValue.Should().Be(false);
        }
        
        [Test] public void TryGetTimeSpan_ReturnsFalse_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetTimeSpan works on Strings
            
            var success = PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetTimeSpan)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetTimeSpan_SetOutArgument_Value_ToNull_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetTimeSpan works on Strings
            
            PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetTimeSpan)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetTimeSpan_ReturnsFalse_WhenSavedKey_CantBeParsedToTimeSpan()
        {
            const string customString = "DEFINITELY_NOT_A_TIMESPAN";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetTimeSpan works on Strings
            
            var success = PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetTimeSpan)}: Could not parse loaded string ({customString})");
        }
        
        [Test] public void TryGetTimeSpan_SetOutArgument_Value_ToNull_WhenSavedKey_CantBeParsedToTimeSpan()
        {
            const string customString = "DEFINITELY_NOT_A_TIMESPAN";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetTimeSpan works on Strings
            
            PlayerPrefsExt.TryGetTimeSpan(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetTimeSpan)}: Could not parse loaded string ({customString})");
        }
    }
}