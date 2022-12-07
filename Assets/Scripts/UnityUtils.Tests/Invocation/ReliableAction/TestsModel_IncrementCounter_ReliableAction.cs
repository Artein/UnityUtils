using System;
using UnityEngine;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    internal class TestsModel_IncrementCounter_ReliableAction : BaseReliableAction
    {
        public TestsModel_IncrementCounter_ReliableAction(TestsModel testsModel, IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) 
            : base(storage, invoker, isFallbackInvocation)
        {
            _testsModel = testsModel;
        }

        public static readonly Guid StaticTypeGuid = new("C3B7E643-9358-4FCF-9337-9BA6403F1F11");
        public override Guid TypeGuid => StaticTypeGuid;
        public int CustomSavableInteger { get; private set; }
        private readonly TestsModel _testsModel;

        public override void Save(string saveKey)
        {
            PlayerPrefs.SetInt(GetCustomSavableIntegerSaveId(saveKey), CustomSavableInteger);
        }

        public override void Load(string saveKey)
        {
            CustomSavableInteger = PlayerPrefs.GetInt(GetCustomSavableIntegerSaveId(saveKey));
        }

        public override void DeleteSave(string saveKey)
        {
            PlayerPrefs.DeleteKey(GetCustomSavableIntegerSaveId(saveKey));
        }

        protected override void Invoke()
        {
            _testsModel.Count += 1;
        }

        private static string GetCustomSavableIntegerSaveId(string saveKey) => $"{saveKey}_{nameof(CustomSavableInteger)}";
    }
}