using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityUtils.Enumeration;
using UnityUtils.Extensions;

namespace UnityUtils.Invocation.ReliableAction
{
    // TODO Reformat: Extract saving/loading feature into separate interfaces (PlayerPrefs not the only option)
    public class ReliableActionsStorage : IReliableActionsStorage
    {
        private readonly List<IReliableAction> _newActions;
        private readonly List<int> _newActionIndicesInFallbackGuids;
        private readonly List<Guid> _fallbackActionGuids;
        private const string BaseSaveKey = "UU_ReliableActionsStorage";
        private const string CountSaveKey = BaseSaveKey + "_Count";
        private readonly List<string> _indexedBaseSaveKeys;
        private readonly List<string> _indexedGuidSaveKeys;
        private readonly StringBuilder _stringBuilder = new();

        public IReadOnlyList<IReliableAction> NewActions => _newActions;

        public ReliableActionsStorage(int capacity = 10)
        {
            _newActions = new List<IReliableAction>(capacity);
            _newActionIndicesInFallbackGuids = new List<int>(capacity);
            _indexedBaseSaveKeys = new List<string>(capacity);
            _indexedGuidSaveKeys = new List<string>(capacity);
            
            LoadAllActionGuids(out _fallbackActionGuids);
        }

        public void Add(IReliableAction action)
        {
            var newActionIndex = AddNewAction(action);

            PlayerPrefs.SetInt(CountSaveKey, _newActions.Count);
            SaveAction(newActionIndex, action);
            PlayerPrefs.Save();
        }

        public bool Remove(IReliableAction action)
        {
            var index = _newActions.IndexOf(action);
            if (index == -1)
            {
                return false;
            }

            // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
            DeleteAllSaves();
            {
                RemoveNewActionAt(index);
            }
            SaveAll();
            return true;
        }

        // TODO: Fix returning actions in backward order
        public IList<IReliableAction> CreateAndTake(IFallbackInvoker invoker, IReliableActionFallbackInstantiator instantiator)
        {
            var takenActions = new List<IReliableAction>();
            
            // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
            DeleteAllSaves();
            {
                // 1. Add newly added actions first
                for (int i = _newActions.Count - 1; i >= 0; i -= 1)
                {
                    var action = _newActions[i];
                    if (ReferenceEquals(action.FallbackInvoker, invoker))
                    {
                        takenActions.Add(action);
                        RemoveNewActionAt(i);
                    }
                }
                
                // 2. Find actions supported by invoker via comparing type-guids
                for (int i = _fallbackActionGuids.Count - 1; i >= 0 ; i -= 1)
                {
                    var typeGuid = _fallbackActionGuids[i];
                    if (invoker.SupportedActionTypes.TryGetValue(typeGuid, out var actionType))
                    {
                        var action = instantiator.Instantiate(actionType);
                        var baseSaveKey = GetActionSaveKey_Base(i);
                        action.Load(baseSaveKey);
                        takenActions.Add(action);
                        _fallbackActionGuids.RemoveAt(i);
                    }
                }
            }
            SaveAll();
            
            return takenActions;
        }

        /// <returns> Index in fallback actions </returns>
        private int AddNewAction(IReliableAction action)
        {
            Assert.IsFalse(_newActions.Contains(action));
            _newActions.Add(action);
            var newActionIndex = _fallbackActionGuids.Count;
            _fallbackActionGuids.Add(action.TypeGuid);
            _newActionIndicesInFallbackGuids.Add(newActionIndex);
            return newActionIndex;
        }

        private void RemoveNewActionAt(int index)
        {
            _newActions.RemoveAt(index);
            var fallbackIndex = _newActionIndicesInFallbackGuids[index];
            _newActionIndicesInFallbackGuids.RemoveAt(index);
            _fallbackActionGuids.RemoveAt(fallbackIndex);
        }

        private void DeleteAllSaves()
        {
            PlayerPrefs.DeleteKey(CountSaveKey);
            
            if (_newActions.Count > 0)
            {
                foreach (var idx in ..(_newActions.Count - 1))
                {
                    DeleteActionSave(idx);
                }
            }
        }

        private void DeleteActionSave(int actionIndex)
        {
            Assert.IsTrue(actionIndex >= 0);
            Assert.IsTrue(actionIndex < _newActions.Count);
            var guidSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefs.DeleteKey(guidSaveKey);
            var reliableAction = _newActions[actionIndex];
            var baseSaveKey = GetActionSaveKey_Base(actionIndex);
            reliableAction.DeleteSave(baseSaveKey);
        }

        private void SaveAll()
        {
            PlayerPrefs.SetInt(CountSaveKey, _newActions.Count);
            for (int i = 0; i < _newActions.Count; i++)
            {
                var action = _newActions[i];
                var fallbackIndex = _newActionIndicesInFallbackGuids[i];
                SaveAction(fallbackIndex, action);
            }
            PlayerPrefs.Save();
        }

        private void SaveAction(int index, [NotNull] IReliableAction action)
        {
            var typeGuidSaveKey = GetActionSaveKey_Guid(index);
            PlayerPrefsExt.SetGuid(typeGuidSaveKey, action.TypeGuid);

            var baseSaveKey = GetActionSaveKey_Base(index);
            action.Save(baseSaveKey);
        }

        private void LoadAllActionGuids(out List<Guid> list)
        {
            var actionsCount = PlayerPrefs.GetInt(CountSaveKey, defaultValue: 0);
            list = new List<Guid>(actionsCount);
            for (var i = 0; i < actionsCount; i += 1)
            {
                var action = LoadActionGuid(i);
                list.Add(action);
            }
        }

        private Guid LoadActionGuid(int index)
        {
            var guidSaveKey = GetActionSaveKey_Guid(index);
            if (PlayerPrefsExt.TryGetGuid(guidSaveKey, out var typeGuid))
            {
                Assert.IsTrue(typeGuid.HasValue);
                return typeGuid.Value;
            }
            
            throw new Exception($"Could not load action by type-guid:{guidSaveKey} at index:{index}");
        }

        private string GetActionSaveKey_Base(int index)
        {
            for (int i = _indexedBaseSaveKeys.Count; i <= index; i += 1)
            {
                _indexedBaseSaveKeys.Add(null);
            }
            
            var saveKey = _indexedBaseSaveKeys[index];
            if (string.IsNullOrEmpty(saveKey))
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(BaseSaveKey);
                _stringBuilder.Append("_Action_");
                _stringBuilder.Append(index);
                saveKey = _stringBuilder.ToString(); // $"{BaseSaveKey}_Action_{index}";
                _indexedBaseSaveKeys[index] = saveKey;
            }
            
            return saveKey;
        }

        private string GetActionSaveKey_Guid(int index)
        {
            for (int i = _indexedGuidSaveKeys.Count; i <= index; i += 1)
            {
                _indexedGuidSaveKeys.Add(null);
            }
            
            var saveKey = _indexedGuidSaveKeys[index];
            if (string.IsNullOrEmpty(saveKey))
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(GetActionSaveKey_Base(index));
                _stringBuilder.Append("_TypeGuid");
                saveKey = _stringBuilder.ToString(); // $"{GetActionSaveKey_Base(index)}_TypeGuid";
                _indexedGuidSaveKeys[index] = saveKey;
            }
            
            return saveKey;
        }
    }
}