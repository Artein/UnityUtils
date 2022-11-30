using System.Collections.Generic;
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
            SaveAction(_actions.Count - 1, action);
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

        [MustUseReturnValue]
        public IList<IReliableAction> Take(IFallbackInvoker invoker)
        {
            var takenActions = new List<IReliableAction>();
            for (int i = 0; i < _actions.Count; i++)
            {
                var action = _actions[i];
                if (ReferenceEquals(action.FallbackInvoker, invoker))
                {
                    takenActions.Add(action);
                }
            }
            
            if (takenActions.Count > 0)
            {
                // TODO Performance: Find better way instead of re-saving everything. Any IO is super slow
                DeleteAllSaves();
                for (int i = 0; i < takenActions.Count; i++)
                {
                    var takenAction = takenActions[i];
                    _actions.Remove(takenAction);
                }
                SaveAll();
            }

            return takenActions;
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

        private void SaveAction(int actionIndex, [NotNull] IReliableAction action)
        {
            var typeGuidSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefsExt.SetGuid(typeGuidSaveKey, action.TypeGuid);

            var baseSaveKey = GetActionSaveKey_Base(actionIndex);
            action.Save(baseSaveKey);
        }

        // TODO GC: Create pool based on action index
        private static string GetActionSaveKey_Base(int actionIndex) => $"{BaseSaveKey}_Action_{actionIndex}";
        private static string GetActionSaveKey_Guid(int actionIndex) => $"{GetActionSaveKey_Base(actionIndex)}_Guid";
    }
}