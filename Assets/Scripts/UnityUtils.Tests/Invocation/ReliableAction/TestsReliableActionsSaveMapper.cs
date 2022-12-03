using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    // We could use some registrar that would parse assembly on start and retrieve all inheritors of IReliableAction,
    // But that would impact runtime performance. Better manually add them once
    public class TestsReliableActionsSaveMapper : IReliableActionsSaveMapper
    {
        private static readonly List<KeyValuePair<Type, Guid>> AllReliableActionTypes = new()
        {
            new KeyValuePair<Type, Guid>(typeof(TestsReliableAction), TestsReliableAction.StaticTypeGuid),
        };
        
        [CanBeNull] public Type FindType(Guid typeGuid)
        {
            for (int i = 0; i < AllReliableActionTypes.Count; i++)
            {
                var pair = AllReliableActionTypes[i];
                if (pair.Value.Equals(typeGuid))
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}