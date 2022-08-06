using System.Collections.Generic;
using Base.UI;
using UnityEngine;
namespace Base {
    public static class B_CoroutineControl {
        
        private static CoroutineRunnerFunctions _coroutineRunnerFunctions;
        public static CoroutineQueue Queue;
        
        public static void Setup(MonoBehaviour parent ,CoroutineRunnerFunctions runnerFunctions) {
            _coroutineRunnerFunctions = runnerFunctions;
            _coroutineRunnerFunctions.SetupCQ(parent);
            _coroutineRunnerFunctions.ManagerStrapping();
            Queue = _coroutineRunnerFunctions.CQ;
        }
        
    }
}