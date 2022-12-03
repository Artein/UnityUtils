using System;
using System.Collections.Generic;
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
        private readonly List<IReliableAction> _actions;
        private readonly List<Guid> _actionTypeGuids;
        private readonly IReliableActionsSaveMapper _saveMapper;
        private readonly IReliableActionInstantiator _instantiator;
        private const string BaseSaveKey = "UU_ReliableActionsStorage";
        private const string CountSaveKey = BaseSaveKey + "_Count";

        public IReadOnlyList<IReliableAction> Actions => _actions;

        public ReliableActionsStorage(IReliableActionsSaveMapper saveMapper, IReliableActionInstantiator instantiator)
        {
            _instantiator = instantiator;
            _saveMapper = saveMapper;

            LoadAllActionGuids(out _actionTypeGuids);
            _actions = new List<IReliableAction>(_actionTypeGuids.Count);
        }

        public void Add(IReliableAction action)
        {
            Assert.IsFalse(_actions.Contains(action));
            _actions.Add(action);

            PlayerPrefs.SetInt(CountSaveKey, _actions.Count);
            SaveAction(_actions.Count - 1, action);
            PlayerPrefs.Save();
        }

        public void Remove(IReliableAction action)
        {
            var index = _actions.IndexOf(action);
            Assert.IsTrue(index >= 0);

            // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
            DeleteAllSaves();
            {
                _actions.RemoveAt(index);
            }
            SaveAll();
        }

        [MustUseReturnValue, CanBeNull]
        public IList<IReliableAction> Take(IFallbackInvoker invoker)
        {
            // TODO: Create actions from _actionTypeGuids here
            // TODO: FallbackInvoker might provide Guid-Types that it handles
            
            int actionsToTake = 0;
            for (int i = 0; i < _actions.Count; i += 1)
            {
                var action = _actions[i];
                if (ReferenceEquals(action.FallbackInvoker, invoker))
                {
                    actionsToTake += 1;
                }
            }

            if (actionsToTake == 0)
            {
                return null;
            }

            var takenActions = new List<IReliableAction>(actionsToTake);
            
            // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
            DeleteAllSaves();
            {
                for (int i = _actions.Count - 1; i >= 0; i -= 1)
                {
                    var action = _actions[i];
                    if (ReferenceEquals(action.FallbackInvoker, invoker))
                    {
                        takenActions.Add(action);
                        _actions.RemoveAt(i);
                    }
                }
            }
            SaveAll();
            
            return takenActions;
        }
        
        private IReliableAction CreateAction(int actionIndex)
        {
            var typeGuid = _actionTypeGuids[actionIndex];
            var actionType = _saveMapper.FindType(typeGuid);
            var action = _instantiator.Instantiate(actionType);
            var baseSaveKey = GetActionSaveKey_Base(actionIndex);
            action.Load(baseSaveKey);
            return action;
        }

        private void DeleteAllSaves()
        {
            PlayerPrefs.DeleteKey(CountSaveKey);
            foreach (var idx in ..(_actions.Count - 1))
            {
                DeleteActionSave(idx);
            }
        }

        private void DeleteActionSave(int actionIndex)
        {
            Assert.IsTrue(actionIndex >= 0);
            Assert.IsTrue(actionIndex < _actions.Count);
            var guidSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefs.DeleteKey(guidSaveKey);
            var reliableAction = _actions[actionIndex];
            var baseSaveKey = GetActionSaveKey_Base(actionIndex);
            reliableAction.DeleteSave(baseSaveKey);
        }

        private void SaveAll()
        {
            PlayerPrefs.SetInt(CountSaveKey, _actions.Count);
            for (int i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                SaveAction(i, action);
            }
            PlayerPrefs.Save();
        }

        private static void SaveAction(int actionIndex, [NotNull] IReliableAction action)
        {
            var typeGuidSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefsExt.SetGuid(typeGuidSaveKey, action.TypeGuid);

            var baseSaveKey = GetActionSaveKey_Base(actionIndex);
            action.Save(baseSaveKey);
        }

        private static void LoadAllActionGuids(out List<Guid> list)
        {
            var actionsCount = PlayerPrefs.GetInt(CountSaveKey, defaultValue: 0);
            list = new List<Guid>(actionsCount);
            for (var i = 0; i < actionsCount; i += 1)
            {
                var action = LoadActionGuid(i);
                list.Add(action);
            }
        }

        private static Guid LoadActionGuid(int actionIdx)
        {
            var guidSaveKey = GetActionSaveKey_Guid(actionIdx);
            if (PlayerPrefsExt.TryGetGuid(guidSaveKey, out var typeGuid))
            {
                Assert.IsTrue(typeGuid.HasValue);
                return typeGuid.Value;
            }
            
            throw new Exception($"Could not load action by type-guid:{guidSaveKey} at index:{actionIdx}");
        }

        // TODO GC: Create pool based on action index
        private static string GetActionSaveKey_Base(int actionIndex) => $"{BaseSaveKey}_Action_{actionIndex}";
        private static string GetActionSaveKey_Guid(int actionIndex) => $"{GetActionSaveKey_Base(actionIndex)}_Guid";
    }
}