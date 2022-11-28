using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityUtils.Extensions;

namespace UnityUtils.Invocation.ReliableAction
{
    public class ReliableActionsStorage : IReliableActionsStorage
    {
        private readonly List<IReliableAction> _actions = new();
        
        private const string BaseSaveKey = "UU_ReliableActionsStorage";
        private const string CountSaveKey = BaseSaveKey + "_Count";
        
        public void Add(IReliableAction action)
        {
            _actions.Add(action);
            PlayerPrefs.SetInt(CountSaveKey, _actions.Count);
            SaveAction(action);
            PlayerPrefs.Save();
        }

        public void Remove(IReliableAction action)
        {
            throw new System.NotImplementedException();
        }

        public IList<IReliableAction> Take(IFallbackInvoker invoker)
        {
            throw new System.NotImplementedException();
        }
        
        private void SaveAction([NotNull] IReliableAction action)
        {
            var actionIndex = _actions.IndexOf(action);
            if (actionIndex < 0)
            {
                throw new InvalidOperationException($"Could not find `{action.GetType().Name}` reliable action at index: {actionIndex}");
            }

            var actionTypeSaveId = action.TypeGuid;
            var actionTypeSaveKey = GetActionSaveKey_Guid(actionIndex);
            PlayerPrefsExt.SetGuid(actionTypeSaveKey, actionTypeSaveId);

            action.Save(GetActionSaveKey_Base(actionIndex));
        }

        private static string GetActionSaveKey_Base(int actionIndex) => $"{BaseSaveKey}_Action_{actionIndex}";
        private static string GetActionSaveKey_Guid(int actionIndex) => $"{GetActionSaveKey_Base(actionIndex)}_Guid";
    }
}