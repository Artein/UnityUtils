using System;
using System.Collections.Generic;
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

        [Test] public void FallbackInvocation_Performs()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator);
                var _ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker);
                
                GC.Collect();
            }
            
            { // Consecutive "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator, false);

                fallbackInvoker.Invoke();

                testsModel.Count.Should().Be(1);
            }
        }
        
        [Test] public void FallbackInvocation_Performs_InScheduledOrder()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator);
                var _ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker, incrementValue: 2);
                var __ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker, incrementValue: 3);
                
                GC.Collect();
            }
            
            { // Consecutive "app run"
                var countChanges = new List<int>(2);
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator, false);

                void CountChanged(int count) => countChanges.Add(count);
                testsModel.CountChanged += CountChanged;
                
                fallbackInvoker.Invoke();

                testsModel.Count.Should().Be(5);
                countChanges.Should().NotBeEmpty()
                    .And.HaveCount(2)
                    .And.ContainInOrder(2, 5);
            }
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