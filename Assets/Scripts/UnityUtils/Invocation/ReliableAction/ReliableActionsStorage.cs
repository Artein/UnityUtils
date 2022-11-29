using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityUtils.Enumeration;
using UnityUtils.Extensions;

namespace UnityUtils.Invocation.ReliableAction
{
    public class ReliableActionsStorage : IReliableActionsStorage
    {
        private readonly List<IReliableAction> _actions = new();
        private const string BaseSaveKey = "UU_ReliableActionsStorage";
        private const string CountSaveKey = BaseSaveKey + "_Count";

        public IReadOnlyList<IReliableAction> Actions => _actions;

        public ReliableActionsStorage()
        {
            // TODO: Load actions from save
        }

        public void Add(IReliableAction action)
        {
            Assert.IsFalse(_actions.Contains(action));
            _actions.Add(action);

            PlayerPrefs.SetInt(CountSaveKey, _actions.Count);
            SaveAction(action);
            PlayerPrefs.Save();
        }

        public void Remove(IReliableAction action)
        {
            Assert.IsTrue(_actions.Contains(action));

            // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
            DeleteAllSaves();
            _actions.Remove(action);
            SaveAll();
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

        [MustUseReturnValue]
        public IList<IReliableAction> Take(IFallbackInvoker invoker)
        {
            // TODO Performance: Get rid of LINQ and GC allocations
            var result = _actions.Where(action => action.FallbackInvoker == invoker).ToList();
            if (result.Count > 0)
            {
                // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
                DeleteAllSaves();
                _actions.RemoveAll(action => action.FallbackInvoker == invoker);
                SaveAll();
            }

            return result;
        }

        private void SaveAction([NotNull] IReliableAction action)
        {
            var actionIndex = _actions.IndexOf(action);
            if (actionIndex < 0)
            {
                throw new InvalidOperationException($"Could not find `{action.GetType().Name}` reliable action at index: {actionIndex}");
            }

            SaveAction(actionIndex, action);
        }

        private void SaveAction(int actionIndex, [NotNull] IReliableAction action)
        {
            var actionTypeSaveId = action.TypeGuid;
            var actionTypeSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefsExt.SetGuid(actionTypeSaveKey, actionTypeSaveId);

            action.Save(GetActionSaveKey_Base(actionIndex));
        }

        // TODO GC: Create pool based on action index
        private static string GetActionSaveKey_Base(int actionIndex) => $"{BaseSaveKey}_Action_{actionIndex}";
        private static string GetActionSaveKey_Guid(int actionIndex) => $"{GetActionSaveKey_Base(actionIndex)}_Guid";
    }
}