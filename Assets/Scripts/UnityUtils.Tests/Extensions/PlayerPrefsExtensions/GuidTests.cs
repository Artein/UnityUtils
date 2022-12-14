using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUtils.Extensions;

namespace Extensions.PlayerPrefsExtensions
{
    [TestFixture] public class GuidTests
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
        
        [Test] public void SetGuid_SavesIntoString()
        {
            Guid value = Guid.NewGuid();
            PlayerPrefsExt.SetGuid(TestsSaveKey, value);

            var savedString = PlayerPrefs.GetString(TestsSaveKey);

            savedString.Should().Be(value.ToString(PlayerPrefsExt.GuidFormat));
        }
        
        [Test] public void TryGetGuid_ReturnsTrue_WhenSuccessfullyLoadedFromSave()
        {
            PlayerPrefsExt.SetGuid(TestsSaveKey, Guid.NewGuid());
            
            var success = PlayerPrefsExt.TryGetGuid(TestsSaveKey, out _);

            success.Should().Be(true);
        }
        
        [Test] public void TryGetGuid_SetOutArgument_Value_WhenSuccessfullyLoadedFromSave()
        {
            Guid saveValue = Guid.NewGuid();
            PlayerPrefsExt.SetGuid(TestsSaveKey, saveValue);
            
            PlayerPrefsExt.TryGetGuid(TestsSaveKey, out var outValue);
            
            outValue!.Value.Should().Be(saveValue);
        }
        
        [Test] public void TryGetGuid_ReturnsFalse_WhenNoKeyWasSaved()
        {
            var success = PlayerPrefsExt.TryGetGuid(TestsSaveKey, out _);

            success.Should().Be(false);
        }
        
        [Test] public void TryGetGuid_SetOutArgument_Value_ToNull_WhenNoKeyWasSaved()
        {
            PlayerPrefsExt.TryGetGuid(TestsSaveKey, out var outValue);

            outValue.HasValue.Should().Be(false);
        }
        
        [Test] public void TryGetGuid_ReturnsFalse_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetGuid works on Strings
            
            var success = PlayerPrefsExt.TryGetGuid(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetGuid)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetGuid_SetOutArgument_Value_ToNull_WhenSavedKey_WasAlreadySaved_WithTypeOtherThanString()
        {
            PlayerPrefs.SetInt(TestsSaveKey, 1); // TryGetGuid works on Strings
            
            PlayerPrefsExt.TryGetGuid(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetGuid)}: Could not parse loaded string ()");
        }
        
        [Test] public void TryGetGuid_ReturnsFalse_WhenSavedKey_CantBeParsedToGuid()
        {
            const string customString = "DEFINITELY_NOT_A_GUID";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetGuid works on Strings
            
            var success = PlayerPrefsExt.TryGetGuid(TestsSaveKey, out _);
            
            success.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetGuid)}: Could not parse loaded string ({customString})");
        }
        
        [Test] public void TryGetGuid_SetOutArgument_Value_ToNull_WhenSavedKey_CantBeParsedToGuid()
        {
            const string customString = "DEFINITELY_NOT_A_GUID";
            PlayerPrefs.SetString(TestsSaveKey, customString); // TryGetGuid works on Strings
            
            PlayerPrefsExt.TryGetGuid(TestsSaveKey, out var outValue);
            
            outValue.HasValue.Should().Be(false);
            LogAssert.Expect(LogType.Error, $"{nameof(PlayerPrefsExt.TryGetGuid)}: Could not parse loaded string ({customString})");
        }
    }
}