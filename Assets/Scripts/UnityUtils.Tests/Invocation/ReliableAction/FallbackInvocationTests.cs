using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    [TestFixture]
    public class FallbackInvocationTests
    {
        [TearDown] public void OnTearDown()
        {
            PlayerPrefs.DeleteAll();
        }
        
        [Test] public void FallbackInvocation_Continues_EvenThoughOneOfActions_ThrowsException()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator);
                var _ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker);
                var __ = new ThrowsExceptionReliableAction(storage, fallbackInvoker);
                
                GC.Collect();
            }

            { // Consecutive "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator, false);

                Action action = () => { fallbackInvoker.Invoke(); };

                action.Should().NotThrow();
                testsModel.Count.Should().Be(1);
            }
        }

        // TODO: Add fallback invocation tests
    }
}