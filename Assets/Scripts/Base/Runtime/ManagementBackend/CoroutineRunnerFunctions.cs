using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Base {
    [Serializable]
    public class CoroutineRunnerFunctions : B_ManagerBase {

        public CoroutineQueue CQ;

        public Task SetupCQ(MonoBehaviour parent) {
            CQ = new CoroutineQueue(parent);
            CQ.StartLoop();
            return Task.CompletedTask;
        }
        
        public override Task ManagerStrapping() {
            
            return base.ManagerStrapping();
        }
        
        public override Task ManagerDataFlush() {
            return base.ManagerDataFlush();
        }
    }
}