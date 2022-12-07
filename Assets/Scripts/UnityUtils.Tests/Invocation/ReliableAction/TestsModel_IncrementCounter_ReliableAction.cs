using System;
using UnityEngine;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    internal sealed class TestsModel_IncrementCounter_ReliableAction : BaseReliableAction<TestsModel_IncrementCounter_ReliableAction.Args>
    {
        public TestsModel_IncrementCounter_ReliableAction(TestsModel testsModel, IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false, int incrementValue = DefaultIncrementValue) 
            : base(storage, invoker, isFallbackInvocation, new Args { IncrementValue = incrementValue })
        {
            _testsModel = testsModel;
        }

        private const int DefaultIncrementValue = 1;
        public static readonly Guid StaticTypeGuid = new("C3B7E643-9358-4FCF-9337-9BA6403F1F11");
        public override Guid TypeGuid => StaticTypeGuid;
        private readonly TestsModel _testsModel;
        public int IncrementValue { get; set; }

        public override void Save(string saveKey)
        {
            PlayerPrefs.SetInt(GetIncrementValueSaveId(saveKey), IncrementValue);
        }

        public override void Load(string saveKey)
        {
            IncrementValue = PlayerPrefs.GetInt(GetIncrementValueSaveId(saveKey), DefaultIncrementValue);
        }

        public override void DeleteSave(string saveKey)
        {
            PlayerPrefs.DeleteKey(GetIncrementValueSaveId(saveKey));
        }

        protected override void OnConstructing(Args args)
        {
            base.OnConstructing(args);
            
            IncrementValue = args.IncrementValue;
        }

        protected override void Invoke()
        {
            _testsModel.Count += IncrementValue;
        }

        private static string GetIncrementValueSaveId(string saveKey) => $"{saveKey}_{nameof(IncrementValue)}";
        
        public struct Args
        {
            public int IncrementValue;
        }
    }
}