using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityUtils.Extensions;

namespace UnityUtils.Invocation.ReliableAction
{
    // TODO Reformat: Extract saving/loading feature into separate interfaces (PlayerPrefs not the only option)
    public class ReliableActionsStorage : IReliableActionsStorage
    {
        private readonly List<IReliableAction> _newActions;
        private readonly List<int> _newActionsSaveSlots;
        private readonly List<Guid> _fallbackActionGuids; // can be indexed by save slots
        private const string BaseSaveKey = "UU_ReliableActionsStorage";
        private const string CountSaveKey = BaseSaveKey + "_Count";
        private readonly List<string> _slottedBaseSaveKeys;
        private readonly List<string> _slottedGuidSaveKeys;
        private readonly StringBuilder _stringBuilder = new(50);

        public IReadOnlyList<IReliableAction> NewActions => _newActions;

        public ReliableActionsStorage(int capacity = 10)
        {
            _newActions = new List<IReliableAction>(capacity);
            _newActionsSaveSlots = new List<int>(capacity);
            _slottedBaseSaveKeys = new List<string>(capacity);
            _slottedGuidSaveKeys = new List<string>(capacity);
            
            LoadAllActionGuids(out _fallbackActionGuids);
        }

        public void Add(IReliableAction action)
        {
            var saveSlot = AddNewAction(action);

            PlayerPrefs.SetInt(CountSaveKey, _fallbackActionGuids.Count);
            SaveAction(saveSlot, action);
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

        public void Clear()
        {
            DeleteAllSaves();
            _newActions.Clear();
            _newActionsSaveSlots.Clear();
            _fallbackActionGuids.Clear();
        }

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
            
            takenActions.Reverse_NoHeapAlloc();
            
            return takenActions;
        }

        /// <returns> Save slot of the fallback actions </returns>
        private int AddNewAction(IReliableAction action)
        {
            Assert.IsFalse(_newActions.Contains(action));
            Assert.AreEqual(_newActions.Count, _newActionsSaveSlots.Count);
            var saveSlot = _fallbackActionGuids.Count;
            
            _newActions.Add(action);
            _fallbackActionGuids.Add(action.TypeGuid);
            _newActionsSaveSlots.Add(saveSlot);
            return saveSlot;
        }

        private void RemoveNewActionAt(int index)
        {
            _newActions.RemoveAt(index);
            var saveSlot = _newActionsSaveSlots[index];
            _newActionsSaveSlots.RemoveAt(index);
            _fallbackActionGuids.RemoveAt(saveSlot);
        }

        private void DeleteAllSaves()
        {
            PlayerPrefs.DeleteKey(CountSaveKey);
            
            if (_newActions.Count > 0)
            {
                for (int i = _newActions.Count - 1; i >= 0; i -= 1)
                {
                    var saveSlot = _newActionsSaveSlots[i];
                    var guidSaveKey = GetActionSaveKey_Guid(saveSlot);
                    PlayerPrefs.DeleteKey(guidSaveKey);
                    
                    var reliableAction = _newActions[i];
                    var baseSaveKey = GetActionSaveKey_Base(saveSlot);
                    reliableAction.DeleteSave(baseSaveKey);
                }

                for (int saveSlot = 0; saveSlot < _fallbackActionGuids.Count; saveSlot += 1)
                {
                    var guidSaveKey = GetActionSaveKey_Guid(saveSlot);
                    PlayerPrefs.DeleteKey(guidSaveKey);
                }
            }
        }

        private void SaveAll()
        {
            PlayerPrefs.SetInt(CountSaveKey, _fallbackActionGuids.Count);
            for (int i = 0; i < _newActions.Count; i += 1)
            {
                var action = _newActions[i];
                var saveSlot = _newActionsSaveSlots[i];
                SaveAction(saveSlot, action);
            }
            PlayerPrefs.Save();
        }

        private void SaveAction(int saveSlot, [NotNull] IReliableAction action)
        {
            var typeGuidSaveKey = GetActionSaveKey_Guid(saveSlot);
            PlayerPrefsExt.SetGuid(typeGuidSaveKey, action.TypeGuid);

            var baseSaveKey = GetActionSaveKey_Base(saveSlot);
            action.Save(baseSaveKey);
        }

        private void LoadAllActionGuids(out List<Guid> list)
        {
            var actionsCount = PlayerPrefs.GetInt(CountSaveKey, defaultValue: 0);
            list = new List<Guid>(actionsCount);
            for (var saveSlot = 0; saveSlot < actionsCount; saveSlot += 1)
            {
                var action = LoadActionGuid(saveSlot);
                list.Add(action);
            }
        }

        private Guid LoadActionGuid(int saveSlot)
        {
            var guidSaveKey = GetActionSaveKey_Guid(saveSlot);
            if (PlayerPrefsExt.TryGetGuid(guidSaveKey, out var typeGuid))
            {
                Assert.IsTrue(typeGuid.HasValue);
                return typeGuid.Value;
            }
            
            throw new Exception($"Could not load action by type-guid:{guidSaveKey} at {nameof(saveSlot)}:{saveSlot}");
        }

        private string GetActionSaveKey_Base(int saveSlot)
        {
            // TODO Performance: If index is large, _indexedBaseSaveKeys might grow several times increasing GC
            // Grow list right away if the diff between Count and Index too big
            while (_slottedBaseSaveKeys.Count <= saveSlot)
            {
                _slottedBaseSaveKeys.Add(null);
            }
            
            var saveKey = _slottedBaseSaveKeys[saveSlot];
            if (string.IsNullOrEmpty(saveKey))
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(BaseSaveKey);
                _stringBuilder.Append("_Action_");
                _stringBuilder.Append(saveSlot);
                saveKey = _stringBuilder.ToString(); // $"{BaseSaveKey}_Action_{saveKey}";
                _slottedBaseSaveKeys[saveSlot] = saveKey;
            }
            
            return saveKey;
        }

        private string GetActionSaveKey_Guid(int saveSlot)
        {
            // TODO Performance: If index is large, _indexedBaseSaveKeys might grow several times increasing GC
            // Grow list right away if the diff between Count and Index too big
            while (_slottedGuidSaveKeys.Count <= saveSlot)
            {
                _slottedGuidSaveKeys.Add(null);
            }
            
            var saveKey = _slottedGuidSaveKeys[saveSlot];
            if (string.IsNullOrEmpty(saveKey))
            {
                _stringBuilder.Clear();
                _stringBuilder.Append(GetActionSaveKey_Base(saveSlot));
                _stringBuilder.Append("_TypeGuid");
                saveKey = _stringBuilder.ToString(); // $"{GetActionSaveKey_Base(saveSlot)}_TypeGuid";
                _slottedGuidSaveKeys[saveSlot] = saveKey;
            }
            
            return saveKey;
        }
    }
}