using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

namespace Base.SoundManagement {
    [Serializable]
    public class SoundManager : B_ManagerBase {
        
        public SoundCollection SoundCollection;
        public Transform SoundParent;

        public override Task ManagerStrapping() {
            Transform musicHolder = new GameObject("SoundHolder").transform;
            musicHolder.SetParent(SoundParent);
            SoundCollection.Initialize(musicHolder);
            return base.ManagerStrapping();
        }

        public override Task ManagerDataFlush() {
            return base.ManagerDataFlush();
        }
        
        public Sound PlaySound(string soundName) {
            return SoundCollection.Play(soundName);
        }
        
        public Sound GetSound(string soundName) {
            return SoundCollection.Get(soundName);
        }
        
    }
}