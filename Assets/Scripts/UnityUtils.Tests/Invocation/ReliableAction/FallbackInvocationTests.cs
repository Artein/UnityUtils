using System;
using System.Collections.Generic;
using FluentAssertions;
using Invocation.ReliableAction.Helpers;
using NUnit.Framework;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    [TestFixture] public class FallbackInvocationTests
    {
        private IReliableActionsStorage _cleanupStorage;

        [TearDown] public void TearDown()
        {
            _cleanupStorage.Clear();
        }
        
        [Test] public void FallbackInvocation_Performs()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator);
                var _ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker);
                
                GC.Collect(0);
            }
            
            { // Consecutive "app run"
                var testsModel = new TestsModel();
                _cleanupStorage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, _cleanupStorage);
                var fallbackInvoker = new TestsFallbackInvoker(_cleanupStorage, fallbackInstantiator);

                fallbackInvoker.Invoke();

                testsModel.Count.Should().Be(1);
            }
        }

        [Test] public void FallbackInvocation_DoesNotPerform_WhenNoParticularActionWasSaved()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                
                var secondFallbackInvoker = new SecondTestsFallbackInvoker(storage, fallbackInstantiator);
                var ___ = new EmptyReliableAction(storage, secondFallbackInvoker);
                
                GC.Collect(0);
            }
            
            { // Consecutive "app run"
                var testsModel = new TestsModel();
                _cleanupStorage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, _cleanupStorage);
                var fallbackInvoker = new TestsFallbackInvoker(_cleanupStorage, fallbackInstantiator);

                fallbackInvoker.Invoke();

                testsModel.Count.Should().Be(0);
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
                
                GC.Collect(0);
            }
            
            { // Consecutive "app run"
                var countChanges = new List<int>(2);
                var testsModel = new TestsModel();
                _cleanupStorage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, _cleanupStorage);
                var fallbackInvoker = new TestsFallbackInvoker(_cleanupStorage, fallbackInstantiator);

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
                
                GC.Collect(0);
            }

            { // Consecutive "app run"
                var testsModel = new TestsModel();
                _cleanupStorage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, _cleanupStorage);
                var fallbackInvoker = new TestsFallbackInvoker(_cleanupStorage, fallbackInstantiator);

                Action action = () => { fallbackInvoker.Invoke(); };

                action.Should().NotThrow();
                testsModel.Count.Should().Be(1);
                Assertion.Expect_LogException_NotImplementedException();
            }
        }

        [Test] public void FallbackInvocation_StatePersists_AcrossMultipleLaunches_WhenSomeInvoker_WasNotInvoked()
        {
            { // First "app run"
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                var fallbackInvoker = new TestsFallbackInvoker(storage, fallbackInstantiator);
                var _ = new TestsModel_IncrementCounter_ReliableAction(testsModel, storage, fallbackInvoker);
                var __ = new ThrowsExceptionReliableAction(storage, fallbackInvoker);
                
                var secondFallbackInvoker = new SecondTestsFallbackInvoker(storage, fallbackInstantiator);
                var ___ = new EmptyReliableAction(storage, secondFallbackInvoker);
                
                GC.Collect(0);
            }
            
            { // Second "app run" — only one invoker invoked
                var testsModel = new TestsModel();
                var storage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, storage);
                
                var secondFallbackInvoker = new SecondTestsFallbackInvoker(storage, fallbackInstantiator);
                secondFallbackInvoker.Invoke();

                GC.Collect(0);
            }
            
            { // Consecutive "app run"
                var testsModel = new TestsModel();
                _cleanupStorage = new ReliableActionsStorage();
                var fallbackInstantiator = new TestsReliableActionFallbackInstantiator(testsModel, _cleanupStorage);
                
                var fallbackInvoker = new TestsFallbackInvoker(_cleanupStorage, fallbackInstantiator);
                fallbackInvoker.Invoke();

                testsModel.Count.Should().Be(1);
                Assertion.Expect_LogException_NotImplementedException();
            }
        }
    }
}