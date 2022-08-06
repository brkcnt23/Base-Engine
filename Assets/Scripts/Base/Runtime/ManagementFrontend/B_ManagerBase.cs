using System.Threading.Tasks;
using UnityEngine;
namespace Base {
    public abstract class B_ManagerBase  {
        
        public virtual async Task ManagerStrapping() {
            return;
        }
        public virtual Task ManagerDataFlush() {
            return Task.CompletedTask;
        }
        
    }
}