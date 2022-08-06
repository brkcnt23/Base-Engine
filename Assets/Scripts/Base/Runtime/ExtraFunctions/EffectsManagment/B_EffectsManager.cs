using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
namespace Base {
    public static class B_EffectsManager {
        public static bool EffectsManagerStrapping() {
            return true;
        }
        
        public static B_PooledParticle SpawnAParticle(this Enum_Particles enumParticles, Vector3 position) {
            return B_EffectsFunctions.instance.SpawnAParticle(enumParticles, position).PlayParticle();
        }
        
        public static B_PooledParticle SpawnAParticle(this Enum_Particles enumParticles, Vector3 position, Quaternion rotation) {
            return B_EffectsFunctions.instance.SpawnAParticle(enumParticles, position, rotation).PlayParticle();
        }
    }
}